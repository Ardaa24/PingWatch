using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingWatch.Data;
using PingWatch.Models;

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
    public async Task<IActionResult> GetAll()
    {
        var ips = await _context.IpAddresses.ToListAsync();
        return Ok(ips);
    }

    [HttpPost]
    public async Task<IActionResult> Add(IpAddress model)
    {
        _context.IpAddresses.Add(model);
        await _context.SaveChangesAsync();
        return Ok(model);
    }
}