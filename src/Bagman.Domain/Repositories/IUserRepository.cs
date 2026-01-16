using Bagman.Domain.Models;
using ErrorOr;

namespace Bagman.Domain.Repositories;

public interface IUserRepository
{
    Task<ErrorOr<User?>> GetByIdAsync(Guid id);
    Task<ErrorOr<User?>> GetByLoginAsync(string login);
    Task<ErrorOr<User?>> GetByEmailAsync(string email);
    Task<ErrorOr<User>> CreateAsync(User user);
    Task<ErrorOr<User>> UpdateAsync(User user);
    // Credential and refresh token operations
    Task<ErrorOr<string?>> GetPasswordHashAsync(Guid userId);
    Task<ErrorOr<Success>> SetPasswordHashAsync(Guid userId, string passwordHash);

    Task<ErrorOr<RefreshToken?>> GetRefreshTokenAsync(string token);
    Task<ErrorOr<Success>> AddRefreshTokenAsync(RefreshToken refreshToken);
    Task<ErrorOr<Success>> RemoveRefreshTokenAsync(string token);
}
