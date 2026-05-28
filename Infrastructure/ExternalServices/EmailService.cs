using System.Net;
using System.Net.Mail;
using PingWatch.Core.Common;
using PingWatch.Core.DTOs.Requests;
using PingWatch.Core.Entities;
using PingWatch.Core.Interfaces.Repositories;
using PingWatch.Core.Interfaces.Services;

namespace PingWatch.Infrastructure.ExternalServices;

/// <summary>
/// SMTP üzerinden e-posta gönderen IEmailService implementasyonu.
/// IServiceProvider kullanımı kaldırıldı — IEmailRepository inject edildi (DIP).
/// </summary>
public class EmailService : IEmailService
{
    private readonly IEmailRepository _emailRepository;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IEmailRepository emailRepository, ILogger<EmailService> logger)
    {
        _emailRepository = emailRepository;
        _logger = logger;
    }

    public async Task<Result<EmailConfig?>> GetConfigAsync(CancellationToken ct = default)
    {
        var config = await _emailRepository.GetConfigAsync(ct);
        return Result<EmailConfig?>.Success(config);
    }

    public async Task<Result> SaveConfigAsync(SaveMailConfigRequest request, CancellationToken ct = default)
    {
        var config = new EmailConfig
        {
            SenderEmail = request.SenderEmail,
            SmtpServer = request.SmtpServer,
            SmtpPort = request.SmtpPort,
            SmtpPassword = request.SmtpPassword
        };
        await _emailRepository.SaveConfigAsync(config, ct);
        await _emailRepository.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result<IEnumerable<EmailRecipient>>> GetRecipientsAsync(CancellationToken ct = default)
    {
        var recipients = await _emailRepository.GetRecipientsAsync(ct);
        return Result<IEnumerable<EmailRecipient>>.Success(recipients);
    }

    public async Task<Result<EmailRecipient>> AddRecipientAsync(AddRecipientRequest request, CancellationToken ct = default)
    {
        var recipient = new EmailRecipient
        {
            FullName = request.FullName,
            EmailAddress = request.EmailAddress
        };
        await _emailRepository.AddRecipientAsync(recipient, ct);
        await _emailRepository.SaveChangesAsync(ct);
        return Result<EmailRecipient>.Success(recipient);
    }

    public async Task<Result> DeleteRecipientAsync(int id, CancellationToken ct = default)
    {
        var recipient = await _emailRepository.GetRecipientByIdAsync(id, ct);
        if (recipient is null)
            return Result.NotFound("Alıcı bulunamadı.");

        await _emailRepository.DeleteRecipientAsync(recipient, ct);
        await _emailRepository.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task SendAlertAsync(string deviceName, string ipAddress, string status, DateTime time, CancellationToken ct = default)
    {
        var config = await _emailRepository.GetConfigAsync(ct);
        var recipients = (await _emailRepository.GetRecipientsAsync(ct)).ToList();

        if (config is null || string.IsNullOrEmpty(config.SmtpServer) || recipients.Count == 0)
        {
            _logger.LogDebug("E-posta ayarları eksik veya alıcı yok. Uyarı gönderilmedi.");
            return;
        }

        try
        {
            using var smtpClient = new SmtpClient(config.SmtpServer)
            {
                Port = config.SmtpPort,
                Credentials = new NetworkCredential(config.SenderEmail, config.SmtpPassword),
                EnableSsl = true
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(config.SenderEmail, "PingWatch Sistem Uyarı"),
                Subject = $"AĞ UYARISI: {deviceName} durumu {status} oldu!",
                Body = $"Cihaz Adı: {deviceName}\nIP Adresi: {ipAddress}\nDurum: {status}\nSaat: {time:dd.MM.yyyy HH:mm:ss}\n\nLütfen kontrol ediniz.",
                IsBodyHtml = false
            };

            foreach (var recipient in recipients)
                mailMessage.To.Add(recipient.EmailAddress);

            await smtpClient.SendMailAsync(mailMessage, ct);
            _logger.LogInformation("Uyarı maili gönderildi. Cihaz: {Device}, Durum: {Status}", deviceName, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mail gönderme hatası. Cihaz: {Device}", deviceName);
        }
    }
}
