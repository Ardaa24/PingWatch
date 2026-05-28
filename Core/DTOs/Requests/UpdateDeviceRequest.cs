using System.ComponentModel.DataAnnotations;

namespace PingWatch.Core.DTOs.Requests;

public class UpdateDeviceRequest
{
    [Required(ErrorMessage = "Cihaz adı zorunludur.")]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "IP adresi veya hostname zorunludur.")]
    [StringLength(255)]
    public string Address { get; set; } = string.Empty;
}
