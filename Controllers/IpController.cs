using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingWatch.Data;
using PingWatch.Models;
using System.Net;

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

    // 📌 IP Listeleme
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var ips = await _context.IpAddresses.ToListAsync();
        return Ok(ips);
    }

    // 📌 IP Ekleme
    [HttpPost]
    public async Task<IActionResult> Add(IpAddress model)
    {
        _context.IpAddresses.Add(model);
        await _context.SaveChangesAsync();
        return Ok(model);
    }

    // 📌 IP Silme
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ip = await _context.IpAddresses.FindAsync(id);
        if (ip == null)
            return NotFound();

        _context.IpAddresses.Remove(ip);
        await _context.SaveChangesAsync();

        return Ok();
    }
}