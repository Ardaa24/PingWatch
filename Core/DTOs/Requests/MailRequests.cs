using System.ComponentModel.DataAnnotations;

namespace PingWatch.Core.DTOs.Requests;

public class SaveMailConfigRequest
{
    [Required(ErrorMessage = "Gönderen e-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    public string SenderEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "SMTP sunucu adresi zorunludur.")]
    public string SmtpServer { get; set; } = string.Empty;

    [Range(1, 65535, ErrorMessage = "Port 1-65535 arasında olmalıdır.")]
    public int SmtpPort { get; set; } = 587;

    [Required(ErrorMessage = "SMTP şifresi zorunludur.")]
    public string SmtpPassword { get; set; } = string.Empty;
}

public class AddRecipientRequest
{
    [Required(ErrorMessage = "Ad Soyad zorunludur.")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    public string EmailAddress { get; set; } = string.Empty;
}
