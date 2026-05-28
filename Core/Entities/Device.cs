namespace PingWatch.Core.Entities;

/// <summary>
/// Sistemde izlenen bir ağ cihazını temsil eder.
/// </summary>
public class Device
{
    public int Id { get; set; }

    /// <summary>Cihazın kullanıcı dostu adı (örn. "Sunucu-1", "Router-Ana")</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Cihazın IPv4 veya IPv6 adresi ya da hostname'i</summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>Son ping sonucuna göre cihazın çevrimiçi durumu</summary>
    public bool IsUp { get; set; } = false;

    /// <summary>İzlemenin aktif olup olmadığı (devre dışı bırakma için)</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Cihazın en son çevrimiçi olduğu zaman</summary>
    public DateTime? LastActiveTime { get; set; }
}
