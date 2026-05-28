using PingWatch.Core.Entities;

namespace PingWatch.Core.Interfaces.Repositories;

/// <summary>Kullanıcı veritabanı erişim sözleşmesi.</summary>
public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default);
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<bool> ExistsWithUsernameAsync(string username, CancellationToken ct = default);
    Task<int> CountAdminsAsync(CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task DeleteAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
