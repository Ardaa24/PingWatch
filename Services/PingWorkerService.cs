using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore;
using PingWatch.Data;

namespace PingWatch.Services;

public class PingWorkerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public PingWorkerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                var ips = await dbContext.IpAddresses.Where(ip => ip.IsActive).ToListAsync(stoppingToken);

                // Tüm IP'lere aynı anda ping atılır. Gecikme sıfıra iner.
                var pingTasks = ips.Select(async ip =>
                {
                    bool isCurrentlyUp = false;
                    try
                    {
                        using var ping = new Ping();
                        var reply = await ping.SendPingAsync(ip.Address, 1500); // 1.5 sn timeout
                        isCurrentlyUp = reply.Status == IPStatus.Success;
                    }
                    catch
                    {
                        isCurrentlyUp = false;
                    }
                    return new { Ip = ip, IsCurrentlyUp = isCurrentlyUp };
                });

                // Tüm paralel işlemlerin bitmesini bekle 
                var results = await Task.WhenAll(pingTasks);

                bool dbChanged = false;

                foreach (var result in results)
                {
                    var ip = result.Ip;
                    bool isCurrentlyUp = result.IsCurrentlyUp;
                    bool wasUpBefore = ip.IsUp;

                    if (wasUpBefore && !isCurrentlyUp) // Çöktü
                    {
                        await emailService.SendAlertAsync(ip.Name, ip.Address, "DOWN (ÇÖKTÜ)", DateTime.Now);
                        ip.IsUp = false;
                        dbChanged = true;
                    }
                    else if (!wasUpBefore && isCurrentlyUp) // Geri Geldi
                    {
                        await emailService.SendAlertAsync(ip.Name, ip.Address, "UP (YENİDEN AKTİF)", DateTime.Now);
                        ip.IsUp = true;
                        ip.LastActiveTime = DateTime.Now;
                        dbChanged = true;
                    }
                    else if (isCurrentlyUp) // Açık Kalmaya Devam Ediyor
                    {
                        ip.LastActiveTime = DateTime.Now;
                        dbChanged = true;
                    }
                }

                if (dbChanged)
                {
                    dbContext.IpAddresses.UpdateRange(results.Select(r => r.Ip));
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }

            // Bekleme süreci artık IP'lere ping atarken geçen süreye bağlı olarak 10 saniyeye kadar azalabilir. Ortalama 1.5-2 saniye sürer.
            await Task.Delay(10000, stoppingToken);
        }
    }
}