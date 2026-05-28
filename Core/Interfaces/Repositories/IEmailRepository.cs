using PingWatch.Core.Entities;

namespace PingWatch.Core.Interfaces.Repositories;

/// <summary>E-posta yapılandırma ve alıcı veritabanı erişim sözleşmesi.</summary>
public interface IEmailRepository
{
    Task<EmailConfig?> GetConfigAsync(CancellationToken ct = default);
    Task SaveConfigAsync(EmailConfig config, CancellationToken ct = default);
    Task<IEnumerable<EmailRecipient>> GetRecipientsAsync(CancellationToken ct = default);
    Task AddRecipientAsync(EmailRecipient recipient, CancellationToken ct = default);
    Task<EmailRecipient?> GetRecipientByIdAsync(int id, CancellationToken ct = default);
    Task DeleteRecipientAsync(EmailRecipient recipient, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
