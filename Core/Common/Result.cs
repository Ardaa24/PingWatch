namespace PingWatch.Core.Common;

/// <summary>
/// Servis katmanından dönen sonucu sarmalayan genel Result pattern implementasyonu.
/// Exception fırlatmak yerine hata bilgisi taşır — controller'lar buna göre HTTP kodu döndürür.
/// </summary>
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public ResultErrorType ErrorType { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        ErrorType = ResultErrorType.None;
    }

    private Result(string error, ResultErrorType errorType)
    {
        IsSuccess = false;
        Error = error;
        ErrorType = errorType;
    }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(string error, ResultErrorType errorType = ResultErrorType.General)
        => new(error, errorType);

    public static Result<T> NotFound(string error = "Kayıt bulunamadı.")
        => new(error, ResultErrorType.NotFound);

    public static Result<T> Conflict(string error)
        => new(error, ResultErrorType.Conflict);

    public static Result<T> Unauthorized(string error = "Yetkisiz erişim.")
        => new(error, ResultErrorType.Unauthorized);
}

/// <summary>
/// Değer taşımayan işlemler için (silme, güncelleme) kullanılan Result tipi.
/// </summary>
public sealed class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public ResultErrorType ErrorType { get; }

    private Result(bool success, string? error = null, ResultErrorType errorType = ResultErrorType.None)
    {
        IsSuccess = success;
        Error = error;
        ErrorType = errorType;
    }

    public static Result Success() => new(true);

    public static Result Failure(string error, ResultErrorType errorType = ResultErrorType.General)
        => new(false, error, errorType);

    public static Result NotFound(string error = "Kayıt bulunamadı.")
        => new(false, error, ResultErrorType.NotFound);

    public static Result Conflict(string error)
        => new(false, error, ResultErrorType.Conflict);

    public static Result Unauthorized(string error = "Yetkisiz erişim.")
        => new(false, error, ResultErrorType.Unauthorized);
}

public enum ResultErrorType
{
    None,
    NotFound,
    Conflict,
    Unauthorized,
    General
}
