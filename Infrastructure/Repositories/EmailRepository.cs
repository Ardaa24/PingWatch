using Microsoft.EntityFrameworkCore;
using PingWatch.Core.Entities;
using PingWatch.Core.Interfaces.Repositories;
using PingWatch.Infrastructure.Data;

namespace PingWatch.Infrastructure.Repositories;

public class EmailRepository : IEmailRepository
{
    private readonly AppDbContext _context;

    public EmailRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EmailConfig?> GetConfigAsync(CancellationToken ct = default)
        => await _context.EmailConfigs.FirstOrDefaultAsync(ct);

    public async Task SaveConfigAsync(EmailConfig config, CancellationToken ct = default)
    {
        var existing = await _context.EmailConfigs.FirstOrDefaultAsync(ct);
        if (existing is null)
        {
            await _context.EmailConfigs.AddAsync(config, ct);
        }
        else
        {
            existing.SenderEmail = config.SenderEmail;
            existing.SmtpServer = config.SmtpServer;
            existing.SmtpPort = config.SmtpPort;
            existing.SmtpPassword = config.SmtpPassword;
        }
    }

    public async Task<IEnumerable<EmailRecipient>> GetRecipientsAsync(CancellationToken ct = default)
        => await _context.EmailRecipients.AsNoTracking().ToListAsync(ct);

    public async Task AddRecipientAsync(EmailRecipient recipient, CancellationToken ct = default)
        => await _context.EmailRecipients.AddAsync(recipient, ct);

    public async Task<EmailRecipient?> GetRecipientByIdAsync(int id, CancellationToken ct = default)
        => await _context.EmailRecipients.FindAsync([id], ct);

    public Task DeleteRecipientAsync(EmailRecipient recipient, CancellationToken ct = default)
    {
        _context.EmailRecipients.Remove(recipient);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
