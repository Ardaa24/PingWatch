using Microsoft.EntityFrameworkCore;
using PingWatch.Core.Entities;
using PingWatch.Infrastructure.Data.Configurations;

namespace PingWatch.Infrastructure.Data;

/// <summary>
/// Uygulama veritabanı bağlamı. 
/// Tüm entity konfigürasyonları ayrı IEntityTypeConfiguration sınıflarında tanımlanmıştır.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Device> Devices { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<EmailConfig> EmailConfigs { get; set; }
    public DbSet<EmailRecipient> EmailRecipients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new DeviceConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new EmailConfigConfiguration());
    }
}
