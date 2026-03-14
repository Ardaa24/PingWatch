using System.Net.NetworkInformation;
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
                var ips = dbContext.IpAddresses.Where(ip => ip.IsActive).ToList();

                foreach (var ip in ips)
                {
                    try
                    {
                        using var ping = new Ping();
                        var reply = await ping.SendPingAsync(ip.Address, 1500); // 1.5 sn timeout

                        bool isCurrentlyUp = reply.Status == IPStatus.Success;
                        bool wasUpBefore = ip.IsUp;

                        // Durum değişti mi Kontrolü (Mail Atma Tetikleyicisi)
                        if (wasUpBefore && !isCurrentlyUp)
                        {
                            // CİHAZ ÇÖKTÜ!
                            await emailService.SendAlertAsync(ip.Name, ip.Address, "DOWN (ÇÖKTÜ)", DateTime.Now);
                        }
                        else if (!wasUpBefore && isCurrentlyUp)
                        {
                            // CİHAZ GERİ GELDİ!
                            await emailService.SendAlertAsync(ip.Name, ip.Address, "UP (YENİDEN AKTİF)", DateTime.Now);
                        }

                        ip.IsUp = isCurrentlyUp;

                        // SON GÖRÜLME MANTIĞI FİX: Sadece cihaz açıksa saati güncelle, çöktüyse son anı koru!
                        if (isCurrentlyUp)
                        {
                            ip.LastActiveTime = DateTime.Now;
                        }

                        dbContext.IpAddresses.Update(ip);
                    }
                    catch
                    {
                        if (ip.IsUp) // Cihaz az önce çöktüyse mail at
                        {
                            await emailService.SendAlertAsync(ip.Name, ip.Address, "DOWN (ULAŞILAMIYOR)", DateTime.Now);
                        }
                        ip.IsUp = false;
                        dbContext.IpAddresses.Update(ip);
                    }
                }
                await dbContext.SaveChangesAsync();
            }
            await Task.Delay(15000, stoppingToken); // 15 saniyede bir tara
        }
    }
}