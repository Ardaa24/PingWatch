namespace PingWatch.Core.Interfaces.Services;

/// <summary>Şifre hash ve doğrulama işlemleri sözleşmesi.</summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash, string algorithm = "BCrypt");
}
