using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingWatch.Core.Common;
using PingWatch.Core.DTOs.Requests;
using PingWatch.Core.Interfaces.Services;

namespace PingWatch.Controllers;

/// <summary>
/// Mail yapılandırma ve alıcı yönetimi.
/// AdminController'dan ayrıştırıldı — SRP: AdminController kullanıcı işlemleri, 
/// MailController mail işlemleri için sorumlu.
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class MailController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IWebHostEnvironment _env;

    public MailController(IEmailService emailService, IWebHostEnvironment env)
    {
        _emailService = emailService;
        _env = env;
    }

    // --- MAIL YÖNETİMİ ---

    [HttpGet("mailconfig")]
    public async Task<IActionResult> GetMailConfig(CancellationToken ct)
    {
        var result = await _emailService.GetConfigAsync(ct);
        return Ok(result.Value);
    }

    [HttpPost("mailconfig")]
    public async Task<IActionResult> SaveMailConfig([FromBody] SaveMailConfigRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.Fail("Geçersiz istek verisi."));

        var result = await _emailService.SaveConfigAsync(request, ct);
        return result.IsSuccess
            ? Ok(ApiResponse.Ok("Mail ayarları kaydedildi."))
            : BadRequest(ApiResponse.Fail(result.Error!));
    }

    [HttpGet("recipients")]
    public async Task<IActionResult> GetRecipients(CancellationToken ct)
    {
        var result = await _emailService.GetRecipientsAsync(ct);
        return Ok(result.Value);
    }

    [HttpPost("recipients")]
    public async Task<IActionResult> AddRecipient([FromBody] AddRecipientRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.Fail("Geçersiz istek verisi."));

        var result = await _emailService.AddRecipientAsync(request, ct);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(result.Value!))
            : BadRequest(ApiResponse.Fail(result.Error!));
    }

    [HttpDelete("recipients/{id:int}")]
    public async Task<IActionResult> DeleteRecipient(int id, CancellationToken ct)
    {
        var result = await _emailService.DeleteRecipientAsync(id, ct);
        return result.IsSuccess
            ? Ok(ApiResponse.Ok("Alıcı silindi."))
            : result.ErrorType switch
            {
                ResultErrorType.NotFound => NotFound(ApiResponse.Fail(result.Error!)),
                _ => BadRequest(ApiResponse.Fail(result.Error!))
            };
    }

    // --- LOGO YÖNETİMİ ---

    [HttpPost("logo")]
    public async Task<IActionResult> UploadLogo(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(ApiResponse.Fail("Dosya seçilmedi."));

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext is not ".png" and not ".jpg" and not ".jpeg")
            return BadRequest(ApiResponse.Fail("Sadece PNG/JPG yüklenebilir."));

        var assetsPath = Path.Combine(_env.WebRootPath, "Assets");
        Directory.CreateDirectory(assetsPath);

        var filePath = Path.Combine(assetsPath, "logo.png");
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, ct);

        return Ok(ApiResponse.Ok("Logo güncellendi."));
    }
}
