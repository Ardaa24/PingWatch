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
        _logger.LogInformation("Ping İzleme Servisi Başladı (Mail Korumalı)...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    // Email Servisini Çağırıyoruz
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                    var ips = context.IpAddresses.Where(ip => ip.IsActive).ToList();

                    var pingTasks = ips.Select(async ip =>
                    {
                        bool previousState = ip.IsUp; // Veritabanındaki eski durum (Örn: TRUE)
                        bool newState = false;        // Ping sonrası yeni durum

                        using (Ping ping = new Ping())
                        {
                            try
                            {
                                PingReply reply = await ping.SendPingAsync(ip.Address, 2000);
                                newState = reply.Status == IPStatus.Success;
                            }
                            catch
                            {
                                newState = false;
                            }
                        }

                        // EĞER DURUM DEĞİŞTİYSE (Örn: True iken False olduysa veya tam tersi)
                        if (previousState != newState)
                        {
                            ip.IsUp = newState; // Veritabanı için durumu güncelle

                            try
                            {
                                // Mail Gönder!
                                await emailService.SendAlertAsync(ip.Name, ip.Address, newState);
                                _logger.LogInformation($"Mail başarıyla gönderildi: {ip.Name} cihazı {newState} durumuna geçti.");
                            }
                            catch (Exception mailEx)
                            {
                                _logger.LogError($"Mail GÖNDERİLEMEDİ: {mailEx.Message}");
                            }
                        }
                    });

                    await Task.WhenAll(pingTasks);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ping döngüsünde kritik bir hata oluştu.");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}