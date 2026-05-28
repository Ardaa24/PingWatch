namespace PingWatch.Core.Entities;

/// <summary>E-posta gönderimi için SMTP sunucu yapılandırması.</summary>
public class EmailConfig
{
    public int Id { get; set; }
    public string SenderEmail { get; set; } = string.Empty;
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpPassword { get; set; } = string.Empty;
}
