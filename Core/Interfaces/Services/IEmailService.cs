using PingWatch.Core.Common;
using PingWatch.Core.DTOs.Requests;
using PingWatch.Core.Entities;

namespace PingWatch.Core.Interfaces.Services;

/// <summary>E-posta uyarı ve yapılandırma iş mantığı sözleşmesi.</summary>
public interface IEmailService
{
    Task<Result<EmailConfig?>> GetConfigAsync(CancellationToken ct = default);
    Task<Result> SaveConfigAsync(SaveMailConfigRequest request, CancellationToken ct = default);
    Task<Result<IEnumerable<EmailRecipient>>> GetRecipientsAsync(CancellationToken ct = default);
    Task<Result<EmailRecipient>> AddRecipientAsync(AddRecipientRequest request, CancellationToken ct = default);
    Task<Result> DeleteRecipientAsync(int id, CancellationToken ct = default);
    Task SendAlertAsync(string deviceName, string ipAddress, string status, DateTime time, CancellationToken ct = default);
}
