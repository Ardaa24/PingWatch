using PingWatch.Core.Entities;

namespace PingWatch.Core.Interfaces.Repositories;

/// <summary>
/// Cihaz (Device) veritabanı erişim sözleşmesi.
/// Concrete implementasyon Infrastructure katmanındadır.
/// </summary>
public interface IDeviceRepository
{
    Task<IEnumerable<Device>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Device>> GetAllActiveAsync(CancellationToken ct = default);
    Task<Device?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsWithAddressOrNameAsync(string address, string name, int? excludeId = null, CancellationToken ct = default);
    Task AddAsync(Device device, CancellationToken ct = default);
    Task UpdateAsync(Device device, CancellationToken ct = default);
    Task UpdateRangeAsync(IEnumerable<Device> devices, CancellationToken ct = default);
    Task DeleteAsync(Device device, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
