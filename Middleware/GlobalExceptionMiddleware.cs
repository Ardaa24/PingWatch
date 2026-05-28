using System.Net;
using System.Text.Json;
using PingWatch.Core.Common;

namespace PingWatch.Middleware;

/// <summary>
/// Tüm unhandled exception'ları yakalar ve standart ApiResponse formatında döndürür.
/// Her controller ayrı ayrı try-catch yazmak zorunda kalmaz (DRY, SRP).
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İşlenmemiş hata. Path: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Production'da internal hata detayını gizle
        var errorMessage = _env.IsDevelopment()
            ? exception.ToString()
            : "Sunucu hatası oluştu. Lütfen daha sonra tekrar deneyin.";

        var response = ApiResponse.Fail(errorMessage);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        await context.Response.WriteAsync(json);
    }
}
