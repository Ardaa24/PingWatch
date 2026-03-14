namespace PingWatch.Models;

public class EmailConfig
{
    public int Id { get; set; }
    public string SenderEmail { get; set; } = string.Empty;
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpPassword { get; set; } = string.Empty;
}

public class EmailRecipient
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
}