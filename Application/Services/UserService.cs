using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PingWatch.Core.Common;
using PingWatch.Core.DTOs.Requests;
using PingWatch.Core.DTOs.Responses;
using PingWatch.Core.Entities;
using PingWatch.Core.Interfaces.Repositories;
using PingWatch.Core.Interfaces.Services;

namespace PingWatch.Application.Services;

/// <summary>JWT ile yapılandırma seçenekleri.</summary>
public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "PingWatch";
    public string Audience { get; set; } = "PingWatchUsers";
    public int ExpiryMinutes { get; set; } = 480; // 8 saat
}

/// <summary>
/// Kullanıcı kimlik doğrulama ve yönetim servisi.
/// JWT token üretir, BCrypt/SHA256 hibrit doğrulama yapar.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtSettings _jwtSettings;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username, ct);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash, user.HashAlgorithm))
            return Result<AuthResponse>.Unauthorized("Hatalı kullanıcı adı veya şifre!");

        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        return Result<AuthResponse>.Success(new AuthResponse
        {
            Token = token,
            Username = user.Username,
            Role = user.Role,
            ExpiresAt = expiresAt
        });
    }

    public async Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync(CancellationToken ct = default)
    {
        var users = await _userRepository.GetAllAsync(ct);
        var responses = users.Select(MapToResponse);
        return Result<IEnumerable<UserResponse>>.Success(responses);
    }

    public async Task<Result<UserResponse>> AddUserAsync(AddUserRequest request, CancellationToken ct = default)
    {
        if (await _userRepository.ExistsWithUsernameAsync(request.Username, ct))
            return Result<UserResponse>.Conflict("Bu kullanıcı adı zaten mevcut.");

        var user = new User
        {
            Username = request.Username.Trim().ToLower(),
            PasswordHash = _passwordHasher.Hash(request.Password),
            HashAlgorithm = "BCrypt",
            Role = request.Role
        };

        await _userRepository.AddAsync(user, ct);
        await _userRepository.SaveChangesAsync(ct);

        return Result<UserResponse>.Success(MapToResponse(user));
    }

    public async Task<Result> DeleteUserAsync(int id, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user is null)
            return Result.NotFound("Kullanıcı bulunamadı.");

        // Son admin koruması
        if (user.Role == "Admin" && await _userRepository.CountAdminsAsync(ct) <= 1)
            return Result.Conflict("Sistemdeki son Admin hesabı silinemez!");

        await _userRepository.DeleteAsync(user, ct);
        await _userRepository.SaveChangesAsync(ct);

        return Result.Success();
    }

    private string GenerateJwtToken(User user)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
        var key = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static UserResponse MapToResponse(User u) => new()
    {
        Id = u.Id,
        Username = u.Username,
        Role = u.Role
    };
}
