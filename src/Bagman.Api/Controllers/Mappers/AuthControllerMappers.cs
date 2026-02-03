using Bagman.Contracts.Models;
using Bagman.Contracts.Models.Auth;
using Bagman.Domain.Models;

namespace Bagman.Api.Controllers.Mappers;

public static class AuthControllerMappers
{
    public static AuthResponse ToAuthResponse(this AuthResult authResult)
    {
        return new AuthResponse
        {
            AccessToken = authResult.AccessToken,
            RefreshToken = authResult.RefreshToken,
            ExpiresAt = authResult.ExpiresAt,
            User = new UserResponse
            {
                Id = authResult.User.Id,
                Login = authResult.User.Login,
                Email = authResult.User.Email,
                CreatedAt = authResult.User.CreatedAt,
                IsActive = authResult.User.IsActive
            }
        };
    }
}
