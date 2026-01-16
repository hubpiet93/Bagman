using Bagman.Domain.Models;
using ErrorOr;

namespace Bagman.Domain.Services;

/// <summary>
///     Draft: High-level authorization provider that will encapsulate authentication,
///     token generation/validation and user operations. Implementations will be
///     based on ASP.NET Core Identity + EF Core (or other provider) and will hide
///     provider details behind this interface.
///     This is a draft — no implementation included. Methods can be extended later.
/// </summary>
public interface IAuthorizationProvider
{
    Task<ErrorOr<AuthResult>> RegisterAsync(string login, string password, string email);

    Task<ErrorOr<AuthResult>> LoginAsync(string login, string password);

    Task<ErrorOr<AuthResult>> RefreshTokensAsync(string refreshToken);

    Task<ErrorOr<Success>> LogoutAsync(string refreshToken);

    /// <summary>
    ///     Validate the provided JWT (signature, expiry, claims). Returns true when valid.
    /// </summary>
    Task<ErrorOr<bool>> ValidateJwtAsync(string jwt);
}
