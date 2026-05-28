namespace PingWatch.Core.Entities;

/// <summary>Uyarı e-postası gönderilecek alıcıyı temsil eder.</summary>
public class EmailRecipient
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
}
