using System.Net;
using System.Net.Mail;

namespace PingWatch.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendAlertAsync(string deviceName, string ipAddress, bool isUp)
    {
        var server = _config["SmtpSettings:Server"];
        var port = int.Parse(_config["SmtpSettings:Port"]!);
        var senderName = _config["SmtpSettings:SenderName"];
        var senderEmail = _config["SmtpSettings:SenderEmail"];
        var password = _config["SmtpSettings:Password"];
        var toEmail = _config["SmtpSettings:ToEmail"];

        // Cihazın durumuna göre başlık ve mesajı ayarla
        string statusText = isUp ? "YENİDEN AKTİF (UP) 🟢" : "ÇÖKTÜ (DOWN) 🔴";
        string subject = $"[PingWatch Alert] {deviceName} - {statusText}";

        string body = $@"Merhaba,

İzlenen ağ cihazlarından birinde durum değişikliği tespit edildi.

Cihaz Adı: {deviceName}
IP Adresi: {ipAddress}
Yeni Durum: {statusText}
Tarih/Saat: {DateTime.Now:dd.MM.yyyy HH:mm:ss}

İyi çalışmalar,
PingWatch Otomatik Bildirim Sistemi - Arda Can Süren";

        using (var client = new SmtpClient(server, port))
        {
            client.Credentials = new NetworkCredential(senderEmail, password);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail!, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mailMessage.To.Add(toEmail!);

            await client.SendMailAsync(mailMessage);
        }
    }
}