using Microsoft.EntityFrameworkCore;
using PingWatch.Models;

namespace PingWatch.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<IpAddress> IpAddresses => Set<IpAddress>();
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();
}