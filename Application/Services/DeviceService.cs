using PingWatch.Core.Common;
using PingWatch.Core.DTOs.Requests;
using PingWatch.Core.DTOs.Responses;
using PingWatch.Core.Entities;
using PingWatch.Core.Interfaces.Repositories;
using PingWatch.Core.Interfaces.Services;

namespace PingWatch.Application.Services;

/// <summary>
/// Cihaz iş mantığı. Controller'dan bağımsız, test edilebilir.
/// DB erişimi IDeviceRepository üzerinden yapılır — SRP, DIP.
/// </summary>
public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;

    public DeviceService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<Result<IEnumerable<DeviceResponse>>> GetAllDevicesAsync(CancellationToken ct = default)
    {
        var devices = await _deviceRepository.GetAllAsync(ct);
        var responses = devices.Select(MapToResponse);
        return Result<IEnumerable<DeviceResponse>>.Success(responses);
    }

    public async Task<Result<DeviceResponse>> AddDeviceAsync(AddDeviceRequest request, CancellationToken ct = default)
    {
        var isDuplicate = await _deviceRepository.ExistsWithAddressOrNameAsync(request.Address, request.Name, ct: ct);
        if (isDuplicate)
            return Result<DeviceResponse>.Conflict("Bu IP adresi veya Cihaz Adı zaten sistemde kayıtlı!");

        var device = new Device
        {
            Name = request.Name.Trim(),
            Address = request.Address.Trim(),
            IsUp = false,
            IsActive = true,
            LastActiveTime = null
        };

        await _deviceRepository.AddAsync(device, ct);
        await _deviceRepository.SaveChangesAsync(ct);

        return Result<DeviceResponse>.Success(MapToResponse(device));
    }

    public async Task<Result<DeviceResponse>> UpdateDeviceAsync(int id, UpdateDeviceRequest request, CancellationToken ct = default)
    {
        var existing = await _deviceRepository.GetByIdAsync(id, ct);
        if (existing is null)
            return Result<DeviceResponse>.NotFound("Cihaz bulunamadı.");

        var isDuplicate = await _deviceRepository.ExistsWithAddressOrNameAsync(request.Address, request.Name, excludeId: id, ct: ct);
        if (isDuplicate)
            return Result<DeviceResponse>.Conflict("Bu IP adresi veya Cihaz Adı başka bir cihazda kullanılıyor!");

        // IP adresi değiştirildiyse durumu sıfırla — ping servisi yeniden tarayacak
        if (!string.Equals(existing.Address, request.Address, StringComparison.OrdinalIgnoreCase))
        {
            existing.IsUp = false;
            existing.LastActiveTime = null;
        }

        existing.Name = request.Name.Trim();
        existing.Address = request.Address.Trim();

        await _deviceRepository.UpdateAsync(existing, ct);
        await _deviceRepository.SaveChangesAsync(ct);

        return Result<DeviceResponse>.Success(MapToResponse(existing));
    }

    public async Task<Result> DeleteDeviceAsync(int id, CancellationToken ct = default)
    {
        var device = await _deviceRepository.GetByIdAsync(id, ct);
        if (device is null)
            return Result.NotFound("Silinmek istenen cihaz bulunamadı.");

        await _deviceRepository.DeleteAsync(device, ct);
        await _deviceRepository.SaveChangesAsync(ct);

        return Result.Success();
    }

    /// <summary>Entity → DTO dönüşümü. Tek yerde tanımlı (DRY).</summary>
    private static DeviceResponse MapToResponse(Device d) => new()
    {
        Id = d.Id,
        Name = d.Name,
        Address = d.Address,
        IsUp = d.IsUp,
        IsActive = d.IsActive,
        LastActiveTime = d.LastActiveTime
    };
}
