using System.Net.NetworkInformation;
using PingWatch.Core.Interfaces.Repositories;
using PingWatch.Core.Interfaces.Services;

namespace PingWatch.Application.Services;

/// <summary>
/// Arka planda sürekli çalışan Ping izleme servisi.
/// IServiceProvider kullanımı kaldırıldı — scoped servisler CreateScope() ile doğru alınıyor.
/// Ping intervali appsettings.json'dan okunur (hard-coded 10000ms → configurable).
/// Console.WriteLine → ILogger ile değiştirildi.
/// </summary>
public class PingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PingBackgroundService> _logger;
    private readonly int _intervalMs;

    public PingBackgroundService(IServiceProvider serviceProvider, ILogger<PingBackgroundService> logger, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _intervalMs = configuration.GetValue<int>("PingSettings:IntervalMs", 10000);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PingBackgroundService başlatıldı. Interval: {Interval}ms", _intervalMs);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PingAllDevicesAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ping döngüsünde beklenmeyen hata.");
            }

            await Task.Delay(_intervalMs, stoppingToken);
        }

        _logger.LogInformation("PingBackgroundService durduruldu.");
    }

    private async Task PingAllDevicesAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var deviceRepo = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var devices = (await deviceRepo.GetAllActiveAsync(stoppingToken)).ToList();
        if (devices.Count == 0) return;

        // Tüm cihazlara paralel ping — gecikmeyi minimize eder
        var pingTasks = devices.Select(async device =>
        {
            bool isCurrentlyUp = false;
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(device.Address, 1500);
                isCurrentlyUp = reply.Status == IPStatus.Success;
            }
            catch
            {
                isCurrentlyUp = false;
            }
            return (Device: device, IsCurrentlyUp: isCurrentlyUp);
        });

        var results = await Task.WhenAll(pingTasks);

        bool dbChanged = false;
        foreach (var (device, isCurrentlyUp) in results)
        {
            bool wasUp = device.IsUp;

            if (wasUp && !isCurrentlyUp)
            {
                _logger.LogWarning("Cihaz DOWN: {Name} ({Address})", device.Name, device.Address);
                await emailService.SendAlertAsync(device.Name, device.Address, "DOWN (ÇÖKTÜ)", DateTime.Now, stoppingToken);
                device.IsUp = false;
                dbChanged = true;
            }
            else if (!wasUp && isCurrentlyUp)
            {
                _logger.LogInformation("Cihaz UP: {Name} ({Address})", device.Name, device.Address);
                await emailService.SendAlertAsync(device.Name, device.Address, "UP (YENİDEN AKTİF)", DateTime.Now, stoppingToken);
                device.IsUp = true;
                device.LastActiveTime = DateTime.Now;
                dbChanged = true;
            }
            else if (isCurrentlyUp)
            {
                device.LastActiveTime = DateTime.Now;
                dbChanged = true;
            }
        }

        if (dbChanged)
        {
            await deviceRepo.UpdateRangeAsync(results.Select(r => r.Device), stoppingToken);
            await deviceRepo.SaveChangesAsync(stoppingToken);
        }
    }
}
