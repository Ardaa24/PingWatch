namespace PingWatch.Core.DTOs.Responses;

/// <summary>
/// Başarılı giriş sonrası döndürülen token ve kullanıcı bilgisi.
/// PasswordHash asla bu DTO'ya dahil edilmez.
/// </summary>
public class AuthResponse
{
    public string Token { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
