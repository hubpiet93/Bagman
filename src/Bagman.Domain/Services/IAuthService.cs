using Bagman.Domain.Models;
using ErrorOr;

namespace Bagman.Domain.Services;

public interface IAuthService
{
    Task<ErrorOr<AuthResult>> RegisterAsync(string login, string password, string email);
    Task<ErrorOr<AuthResult>> LoginAsync(string login, string password);
    Task<ErrorOr<AuthResult>> RefreshTokenAsync(string refreshToken);
    Task<ErrorOr<Success>> LogoutAsync(string refreshToken);
}
