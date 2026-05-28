namespace PingWatch.Core.Common;

/// <summary>
/// Tüm API endpoint'lerinin döndürdüğü standart yanıt zarfı.
/// Frontend her zaman tutarlı bir format bekleyebilir.
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }

    public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data };
    public static ApiResponse<T> Fail(string error) => new() { Success = false, Error = error };
}

/// <summary>Veri taşımayan yanıtlar için (204 No Content benzeri başarı)</summary>
public class ApiResponse
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public string? Message { get; init; }

    public static ApiResponse Ok(string? message = null) => new() { Success = true, Message = message };
    public static ApiResponse Fail(string error) => new() { Success = false, Error = error };
}
