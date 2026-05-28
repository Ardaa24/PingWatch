using PingWatch.Core.Entities;
using PingWatch.Core.Interfaces.Services;

namespace PingWatch.Infrastructure.Data.Seed;

/// <summary>
/// Veritabanı başlangıç verilerini oluşturur.
/// Program.cs'den ayrıştırılmıştır — SRP prensibi gereği.
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher hasher)
    {
        // Migration'lar otomatik uygulanır
        await context.Database.EnsureCreatedAsync();

        // Admin hesabı yoksa oluştur (BCrypt ile)
        if (!context.Users.Any(u => u.Username == "admin"))
        {
            context.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = hasher.Hash("admin123"),
                HashAlgorithm = "BCrypt",
                Role = "Admin"
            });
        }

        // Viewer hesabı yoksa oluştur
        if (!context.Users.Any(u => u.Username == "viewer"))
        {
            context.Users.Add(new User
            {
                Username = "viewer",
                PasswordHash = hasher.Hash("viewer123"),
                HashAlgorithm = "BCrypt",
                Role = "Viewer"
            });
        }

        await context.SaveChangesAsync();
    }
}
