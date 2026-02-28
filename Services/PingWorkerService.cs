using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PingWatch.Data;
using System.Net.NetworkInformation;

namespace PingWatch.Services;

public class PingWorkerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PingWorkerService> _logger;

    public PingWorkerService(IServiceProvider serviceProvider, ILogger<PingWorkerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Ping İzleme Servisi Başladı (Paralel Mod)...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var ips = context.IpAddresses.Where(ip => ip.IsActive).ToList();

                    // Paralel Ping İşlemi (Tüm pingleri aynı anda başlat)
                    var pingTasks = ips.Select(async ip =>
                    {
                        using (Ping ping = new Ping())
                        {
                            try
                            {
                                PingReply reply = await ping.SendPingAsync(ip.Address, 2000);
                                ip.IsUp = reply.Status == IPStatus.Success;
                            }
                            catch
                            {
                                ip.IsUp = false; // Hata veya Timeout olursa Down say
                            }
                        }
                    });

                    // Tüm ping işlemlerinin bitmesini bekle
                    await Task.WhenAll(pingTasks);

                    // Sonuçları veritabanına kaydet
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ping işlemi sırasında arka planda bir hata oluştu.");
            }

            // Döngüyü 30 saniye beklet
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}