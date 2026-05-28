using PingWatch.Core.Common;
using PingWatch.Core.DTOs.Requests;
using PingWatch.Core.DTOs.Responses;

namespace PingWatch.Core.Interfaces.Services;

/// <summary>Cihaz iş mantığı sözleşmesi.</summary>
public interface IDeviceService
{
    Task<Result<IEnumerable<DeviceResponse>>> GetAllDevicesAsync(CancellationToken ct = default);
    Task<Result<DeviceResponse>> AddDeviceAsync(AddDeviceRequest request, CancellationToken ct = default);
    Task<Result<DeviceResponse>> UpdateDeviceAsync(int id, UpdateDeviceRequest request, CancellationToken ct = default);
    Task<Result> DeleteDeviceAsync(int id, CancellationToken ct = default);
}
