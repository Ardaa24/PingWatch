using Microsoft.AspNetCore.Mvc;
using PingWatch.Core.Common;
using PingWatch.Core.DTOs.Requests;
using PingWatch.Core.Interfaces.Services;

namespace PingWatch.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>Kullanıcı girişi. Başarılı olursa JWT token döndürür.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.Fail("Geçersiz istek."));

        var result = await _userService.LoginAsync(request, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(result.Value!))
            : Unauthorized(ApiResponse.Fail(result.Error!));
    }
}