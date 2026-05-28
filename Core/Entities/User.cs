namespace PingWatch.Core.Entities;

/// <summary>Kullanıcı rollerini tanımlar.</summary>
public enum UserRole
{
    Admin,
    Viewer
}

/// <summary>
/// Sisteme giriş yapabilen bir kullanıcıyı temsil eder.
/// </summary>
public class User
{
    public int Id { get; set; }

    /// <summary>Benzersiz kullanıcı adı (küçük harf, boşluksuz)</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>BCrypt veya SHA256 ile hashlenmiş şifre</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Şifre hash algoritmasını belirtir. 
    /// "SHA256" eski hesaplar, "BCrypt" yeni hesaplar.
    /// </summary>
    public string HashAlgorithm { get; set; } = "BCrypt";

    public string Role { get; set; } = nameof(UserRole.Admin);
}
