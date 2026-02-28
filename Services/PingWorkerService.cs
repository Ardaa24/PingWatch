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
        _logger.LogInformation("Ping İzleme Servisi Başladı...");

        // Uygulama kapanana kadar bu döngü çalışmaya devam edecek
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // DbContext'i (veritabanı bağlantısı) arka planda kullanabilmek için yeni bir Scope (kapsam) oluşturmalıyız
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // Sadece aktif olan IP'leri getir
                    var ips = context.IpAddresses.Where(ip => ip.IsActive).ToList();

                    foreach (var ip in ips)
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
                                ip.IsUp = false; // Hata verirse veya ulaşılamazsa Down say
                            }
                        }
                    }

                    // Değişiklikleri veritabanına kaydet
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ping işlemi sırasında arka planda bir hata oluştu.");
            }

            // Döngüyü 30 saniye beklet (30 saniyede bir kontrol et)
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}