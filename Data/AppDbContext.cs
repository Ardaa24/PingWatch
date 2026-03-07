using Microsoft.EntityFrameworkCore;
using PingWatch.Models;

namespace PingWatch.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<IpAddress> IpAddresses { get; set; }

    public DbSet<User> Users { get; set; }
}