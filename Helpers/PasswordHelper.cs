using System.Security.Cryptography;
using System.Text;

namespace PingWatch.Helpers;

public static class PasswordHelper
{
    // Düz metin şifreyi SHA256 ile geri döndürülemez bir karma metne çevir
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    // Kullanıcının girdiği şifre ile DB'deki hash'i karşılaştır
    public static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}