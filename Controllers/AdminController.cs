using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingWatch.Core.Common;
using PingWatch.Core.DTOs.Requests;
using PingWatch.Core.Interfaces.Services;

namespace PingWatch.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    // --- KULLANICI YÖNETİMİ ---

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(CancellationToken ct)
    {
        var result = await _userService.GetAllUsersAsync(ct);
        return Ok(result.Value);
    }

    [HttpPost("users")]
    public async Task<IActionResult> AddUser([FromBody] AddUserRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.Fail("Geçersiz istek verisi."));

        var result = await _userService.AddUserAsync(request, ct);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(result.Value!))
            : result.ErrorType switch
            {
                ResultErrorType.Conflict => Conflict(ApiResponse.Fail(result.Error!)),
                _ => BadRequest(ApiResponse.Fail(result.Error!))
            };
    }

    [HttpDelete("users/{id:int}")]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken ct)
    {
        var result = await _userService.DeleteUserAsync(id, ct);
        return result.IsSuccess
            ? Ok(ApiResponse.Ok("Kullanıcı silindi."))
            : result.ErrorType switch
            {
                ResultErrorType.NotFound => NotFound(ApiResponse.Fail(result.Error!)),
                ResultErrorType.Conflict => Conflict(ApiResponse.Fail(result.Error!)),
                _ => BadRequest(ApiResponse.Fail(result.Error!))
            };
    }
}