namespace PingWatch.Core.DTOs.Responses;

/// <summary>
/// Cihaz verilerini dışarıya açan response DTO.
/// Entity'nin tüm alanlarını kopyalamak yerine yalnızca gerekli olanları döndürür.
/// </summary>
public class DeviceResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public bool IsUp { get; init; }
    public bool IsActive { get; init; }
    public DateTime? LastActiveTime { get; init; }
}
