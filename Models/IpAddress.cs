namespace PingWatch.Models;

public class IpAddress
{
    public int Id { get; set; }

    // Cihaz ismini ekledim.
    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public bool IsUp { get; set; } = false;

    public bool IsActive { get; set; } = true;
}