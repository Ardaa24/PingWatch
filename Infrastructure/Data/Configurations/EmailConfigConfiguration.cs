using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingWatch.Core.Entities;

namespace PingWatch.Infrastructure.Data.Configurations;

public class EmailConfigConfiguration : IEntityTypeConfiguration<EmailConfig>
{
    public void Configure(EntityTypeBuilder<EmailConfig> builder)
    {
        builder.ToTable("EmailConfigs");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.SenderEmail).IsRequired().HasMaxLength(200);
        builder.Property(e => e.SmtpServer).IsRequired().HasMaxLength(200);
        builder.Property(e => e.SmtpPassword).HasMaxLength(500);
    }
}
