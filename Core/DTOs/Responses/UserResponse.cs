namespace PingWatch.Core.DTOs.Responses;

/// <summary>
/// Kullanıcı bilgisini dışarıya açar.
/// PasswordHash ve HashAlgorithm asla expose edilmez.
/// </summary>
public class UserResponse
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
