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
        return Ok(ips);
    }

    [HttpPost]
    public async Task<IActionResult> Post(IpAddress ip)
    {
        // Çift Kayıt Engeli (Eklediğim en önemli özellik.)
        bool isDuplicate = await _context.IpAddresses.AnyAsync(x =>
            x.Address.ToLower() == ip.Address.ToLower() ||
            x.Name.ToLower() == ip.Name.ToLower());

        if (isDuplicate)
        {
            return BadRequest("Bu IP adresi veya Cihaz Adı zaten sistemde kayıtlı!");
        }

        ip.LastActiveTime = null; // İlk eklenişte bilinmiyor
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