using Microsoft.EntityFrameworkCore;
using PingWatch.Core.Entities;
using PingWatch.Core.Interfaces.Repositories;
using PingWatch.Infrastructure.Data;

namespace PingWatch.Infrastructure.Repositories;

/// <summary>
/// IDeviceRepository'nin EF Core implementasyonu.
/// Tüm DB erişim mantığı buradadır — controller ve service'ler DB'den habersizdir.
/// </summary>
public class DeviceRepository : IDeviceRepository
{
    private readonly AppDbContext _context;

    public DeviceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Device>> GetAllAsync(CancellationToken ct = default)
        => await _context.Devices.AsNoTracking().ToListAsync(ct);

    public async Task<IEnumerable<Device>> GetAllActiveAsync(CancellationToken ct = default)
        => await _context.Devices.Where(d => d.IsActive).ToListAsync(ct);

    public async Task<Device?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Devices.FindAsync([id], ct);

    /// <summary>
    /// Aynı adres veya isim başka bir cihazda kullanılıyor mu?
    /// DRY: Bu mantık daha önce Post ve Put endpoint'lerinde ayrı ayrı yazılıyordu.
    /// </summary>
    public async Task<bool> ExistsWithAddressOrNameAsync(string address, string name, int? excludeId = null, CancellationToken ct = default)
        => await _context.Devices.AnyAsync(d =>
            d.Id != (excludeId ?? -1) &&
            (d.Address.ToLower() == address.ToLower() || d.Name.ToLower() == name.ToLower()),
            ct);

    public async Task AddAsync(Device device, CancellationToken ct = default)
        => await _context.Devices.AddAsync(device, ct);

    public Task UpdateAsync(Device device, CancellationToken ct = default)
    {
        _context.Devices.Update(device);
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<Device> devices, CancellationToken ct = default)
    {
        _context.Devices.UpdateRange(devices);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Device device, CancellationToken ct = default)
    {
        _context.Devices.Remove(device);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
