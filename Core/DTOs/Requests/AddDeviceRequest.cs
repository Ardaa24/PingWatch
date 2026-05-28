using System.ComponentModel.DataAnnotations;

namespace PingWatch.Core.DTOs.Requests;

public class AddDeviceRequest
{
    [Required(ErrorMessage = "Cihaz adı zorunludur.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Cihaz adı 1-100 karakter arasında olmalıdır.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "IP adresi veya hostname zorunludur.")]
    [StringLength(255, ErrorMessage = "Adres çok uzun.")]
    public string Address { get; set; } = string.Empty;
}
