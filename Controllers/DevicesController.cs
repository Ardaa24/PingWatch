using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingWatch.Core.Common;
using PingWatch.Core.DTOs.Requests;
using PingWatch.Core.Interfaces.Services;

namespace PingWatch.Controllers;

[ApiController]
[Route("api/devices")]
[Authorize]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _deviceService.GetAllDevicesAsync(ct);
        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Add([FromBody] AddDeviceRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.Fail("Geçersiz istek verisi."));

        var result = await _deviceService.AddDeviceAsync(request, ct);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(result.Value!))
            : result.ErrorType switch
            {
                ResultErrorType.Conflict => Conflict(ApiResponse.Fail(result.Error!)),
                _ => BadRequest(ApiResponse.Fail(result.Error!))
            };
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDeviceRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.Fail("Geçersiz istek verisi."));

        var result = await _deviceService.UpdateDeviceAsync(id, request, ct);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(result.Value!))
            : result.ErrorType switch
            {
                ResultErrorType.NotFound => NotFound(ApiResponse.Fail(result.Error!)),
                ResultErrorType.Conflict => Conflict(ApiResponse.Fail(result.Error!)),
                _ => BadRequest(ApiResponse.Fail(result.Error!))
            };
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _deviceService.DeleteDeviceAsync(id, ct);
        return result.IsSuccess
            ? Ok(ApiResponse.Ok("Cihaz başarıyla silindi."))
            : result.ErrorType switch
            {
                ResultErrorType.NotFound => NotFound(ApiResponse.Fail(result.Error!)),
                _ => BadRequest(ApiResponse.Fail(result.Error!))
            };
    }
}
