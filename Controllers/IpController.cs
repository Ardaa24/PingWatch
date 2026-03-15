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
    public async Task<IActionResult> Put(int id, [FromBody] IpAddress updatedIp)
    {
        var existing = await _context.IpAddresses.FindAsync(id);
        if (existing == null) return NotFound("Cihaz bulunamadı.");

        // Çakışma Kontrolü: Kendi ID'si hariç başka bir cihaz bu isim/IP'yi kullanıyor mu?
        bool isDuplicate = await _context.IpAddresses.AnyAsync(x =>
            x.Id != id &&
            (x.Address.ToLower() == updatedIp.Address.ToLower() ||
             x.Name.ToLower() == updatedIp.Name.ToLower()));

        if (isDuplicate)
        {
            return BadRequest("Bu IP adresi veya Cihaz Adı başka bir cihazda kullanılıyor!");
        }

        // Eğer IP adresi değiştirildiyse, durumu sıfırla ki Ping servisi yeniden tarasın
        if (existing.Address != updatedIp.Address)
        {
            existing.IsUp = false;
            existing.LastActiveTime = null;
        }

        existing.Name = updatedIp.Name;
        existing.Address = updatedIp.Address;

        _context.IpAddresses.Update(existing);
        await _context.SaveChangesAsync();
        return Ok(existing);
    }
}