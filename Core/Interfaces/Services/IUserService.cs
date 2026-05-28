using PingWatch.Core.Common;
using PingWatch.Core.DTOs.Requests;
using PingWatch.Core.DTOs.Responses;

namespace PingWatch.Core.Interfaces.Services;

/// <summary>Kullanıcı iş mantığı ve kimlik doğrulama sözleşmesi.</summary>
public interface IUserService
{
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync(CancellationToken ct = default);
    Task<Result<UserResponse>> AddUserAsync(AddUserRequest request, CancellationToken ct = default);
    Task<Result> DeleteUserAsync(int id, CancellationToken ct = default);
}
