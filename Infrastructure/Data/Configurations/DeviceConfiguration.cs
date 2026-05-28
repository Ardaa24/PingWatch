using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingWatch.Core.Entities;

namespace PingWatch.Infrastructure.Data.Configurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Address)
            .IsRequired()
            .HasMaxLength(255);

        // Aynı isim veya adres iki kez kayıt edilemez
        builder.HasIndex(d => d.Address).IsUnique();
        builder.HasIndex(d => d.Name).IsUnique();

        builder.Property(d => d.IsUp).HasDefaultValue(false);
        builder.Property(d => d.IsActive).HasDefaultValue(true);
    }
}
