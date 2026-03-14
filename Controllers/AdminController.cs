using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingWatch.Data;
using PingWatch.Models;
using PingWatch.Helpers;

namespace PingWatch.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public AdminController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // --- KULLANICI YÖNETİMİ ---
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers() => Ok(await _context.Users.Select(u => new { u.Id, u.Username, u.Role }).ToListAsync());

    [HttpPost("users")]
    public async Task<IActionResult> AddUser([FromBody] User newUser)
    {
        if (await _context.Users.AnyAsync(u => u.Username == newUser.Username))
            return BadRequest("Bu kullanıcı adı zaten var.");

        newUser.PasswordHash = PasswordHelper.HashPassword(newUser.PasswordHash); // Frontend'den gelen düz şifreyi hashle
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();
        if (user.Username == "admin" && await _context.Users.CountAsync(u => u.Role == "Admin") <= 1)
            return BadRequest("Sistemdeki son Admin hesabı silinemez!");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return Ok();
    }

    // --- MAİL YÖNETİMİ ---
    [HttpGet("mailconfig")]
    public async Task<IActionResult> GetMailConfig() => Ok(await _context.EmailConfigs.FirstOrDefaultAsync());

    [HttpPost("mailconfig")]
    public async Task<IActionResult> SaveMailConfig([FromBody] EmailConfig config)
    {
        var existing = await _context.EmailConfigs.FirstOrDefaultAsync();
        if (existing == null) _context.EmailConfigs.Add(config);
        else { existing.SenderEmail = config.SenderEmail; existing.SmtpServer = config.SmtpServer; existing.SmtpPort = config.SmtpPort; existing.SmtpPassword = config.SmtpPassword; }
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("recipients")]
    public async Task<IActionResult> GetRecipients() => Ok(await _context.EmailRecipients.ToListAsync());

    [HttpPost("recipients")]
    public async Task<IActionResult> AddRecipient([FromBody] EmailRecipient rec)
    {
        _context.EmailRecipients.Add(rec);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("recipients/{id}")]
    public async Task<IActionResult> DeleteRecipient(int id)
    {
        var rec = await _context.EmailRecipients.FindAsync(id);
        if (rec != null) { _context.EmailRecipients.Remove(rec); await _context.SaveChangesAsync(); }
        return Ok();
    }

    // --- LOGO YÜKLEME YÖNETİMİ ---
    [HttpPost("logo")]
    public async Task<IActionResult> UploadLogo(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("Dosya seçilmedi.");
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (ext != ".png" && ext != ".jpg" && ext != ".jpeg") return BadRequest("Sadece PNG/JPG yüklenebilir.");

        // wwwroot/Assets klasörünü kontrol et, yoksa oluştur
        var assetsPath = Path.Combine(_env.WebRootPath, "Assets");
        if (!Directory.Exists(assetsPath)) Directory.CreateDirectory(assetsPath);

        var filePath = Path.Combine(assetsPath, "logo.png"); // Sabit isimle üstüne yazar
        using (var stream = new FileStream(filePath, FileMode.Create)) { await file.CopyToAsync(stream); }
        return Ok();
    }
}