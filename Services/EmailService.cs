using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using PingWatch.Data;

namespace PingWatch.Services;

public class EmailService
{
    private readonly IServiceProvider _serviceProvider;

    public EmailService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SendAlertAsync(string deviceName, string ipAddress, string status, DateTime time)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var config = await db.EmailConfigs.FirstOrDefaultAsync();
        var recipients = await db.EmailRecipients.ToListAsync();

        if (config == null || string.IsNullOrEmpty(config.SmtpServer) || !recipients.Any())
            return; // Ayar veya alıcı yoksa mail atma

        try
        {
            var smtpClient = new SmtpClient(config.SmtpServer)
            {
                Port = config.SmtpPort,
                Credentials = new NetworkCredential(config.SenderEmail, config.SmtpPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(config.SenderEmail, "PingWatch Sistem Uyarı"),
                Subject = $"AĞ UYARISI: {deviceName} durumu {status} oldu!",
                Body = $"Cihaz Adı: {deviceName}\nIP Adresi: {ipAddress}\nDurum: {status}\nSaat: {time.ToString("dd.MM.yyyy HH:mm:ss")}\n\nLütfen kontrol ediniz.",
                IsBodyHtml = false,
            };

            foreach (var recipient in recipients)
            {
                mailMessage.To.Add(recipient.EmailAddress);
            }

            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Mail Gönderme Hatası: {ex.Message}");
        }
    }
}