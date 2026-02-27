using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingWatch.Data;
using PingWatch.Models;
using System.Net.NetworkInformation;
namespace PingWatch.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IpController : ControllerBase
{
    private readonly AppDbContext _context;

    public IpController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetIps()
    {
        var ips = await _context.IpAddresses.ToListAsync();

        foreach (var ip in ips)
        {
            using (Ping ping = new Ping())
            {
                try
                {
                    // Timeout süresi eklemek (örn: 1000ms) her zaman daha iyidir
                    PingReply reply = await ping.SendPingAsync(ip.Address, 1000);
                    ip.IsUp = (reply.Status == IPStatus.Success);
                }
                catch (Exception)
                {
                    // Bir hata oluşursa (adrese ulaşılamazsa) cihazı kapalı say
                    ip.IsUp = false;
                }
            }
        }

        await _context.SaveChangesAsync();
        return Ok(ips);
    }

    [HttpPost]
    public async Task<IActionResult> AddIp(IpAddress ip)
    {
        Ping ping = new Ping();
        PingReply reply = await ping.SendPingAsync(ip.Address);

        ip.IsUp = reply.Status == IPStatus.Success;

        _context.IpAddresses.Add(ip);
        await _context.SaveChangesAsync();

        return Ok(ip);
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIp(int id)
    {
        var ip = await _context.IpAddresses.FindAsync(id);

        if (ip == null)
            return NotFound(new { message = "Silinmek istenen IP bulunamadı." });

        try
        {
            _context.IpAddresses.Remove(ip);

            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return Ok(new { message = "Başarıyla silindi." });
            else
                return BadRequest("Silme işlemi veritabanına yansıtılamadı.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Sunucu hatası: {ex.Message}");
        }
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIp(int id, IpAddress updatedIp)
    {
        var ip = await _context.IpAddresses.FindAsync(id);
        if (ip == null)
            return NotFound();

        ip.Address = updatedIp.Address;
        ip.IsActive = updatedIp.IsActive;

        await _context.SaveChangesAsync();

        return Ok(ip);
    }
}