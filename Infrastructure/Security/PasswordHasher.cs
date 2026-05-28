using System.Security.Cryptography;
using System.Text;
using PingWatch.Core.Interfaces.Services;

namespace PingWatch.Infrastructure.Security;

/// <summary>
/// SHA256 (legacy) ve BCrypt (yeni) destekli hibrit şifre hasher.
/// 
/// Strateji: Mevcut SHA256 hesaplar kırılmadan korunur.
/// Giriş yaparken kullanıcının hash algoritması kontrol edilir.
/// Yeni hesaplar her zaman BCrypt ile oluşturulur.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Yeni şifreler için BCrypt kullanır (salt otomatik dahil).
    /// </summary>
    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    /// <summary>
    /// SHA256 veya BCrypt algoritmasına göre doğrulama yapar.
    /// </summary>
    public bool Verify(string password, string hash, string algorithm = "BCrypt")
    {
        return algorithm switch
        {
            "SHA256" => VerifySha256(password, hash),
            "BCrypt" => BCrypt.Net.BCrypt.Verify(password, hash),
            _ => false
        };
    }

    private static bool VerifySha256(string password, string hash)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var computed = BitConverter.ToString(bytes).Replace("-", "").ToLower();
        return computed == hash;
    }
}
