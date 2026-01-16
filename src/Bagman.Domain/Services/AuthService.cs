using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using ErrorOr;
using Result = ErrorOr.Result;

namespace Bagman.Domain.Services;

public class AuthService : IAuthService
{
    private readonly IAuthorizationProvider _authorizationProvider;
    private readonly IUserRepository _userRepository;

    public AuthService(IAuthorizationProvider authorizationProvider, IUserRepository userRepository)
    {
        _authorizationProvider = authorizationProvider;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<AuthResult>> RegisterAsync(string login, string password, string email)
    {
        var existingUserResult = await _userRepository.GetByLoginAsync(login);
        if (existingUserResult.IsError)
            return existingUserResult.Errors;

        if (existingUserResult.Value is not null)
            return Error.Conflict("User.AlreadyExists", "Użytkownik o podanym loginie już istnieje");

        var existingEmailResult = await _userRepository.GetByEmailAsync(email);
        if (existingEmailResult.IsError)
            return existingEmailResult.Errors;

        if (existingEmailResult.Value is not null)
            return Error.Conflict("User.EmailAlreadyExists", "Użytkownik o podanym emailu już istnieje");

        var authResult = await _authorizationProvider.RegisterAsync(login, password, email);
        if (authResult.IsError)
            return authResult.Errors;

        return authResult.Value;
    }

    public async Task<ErrorOr<AuthResult>> LoginAsync(string login, string password)
    {
        var authResult = await _authorizationProvider.LoginAsync(login, password);
        if (authResult.IsError)
            return authResult.Errors;

        var userResult = await _userRepository.GetByIdAsync(authResult.Value.User.Id);
        if (userResult.IsError)
            return userResult.Errors;

        if (userResult.Value is null)
            return Error.NotFound("User.NotFound", "Użytkownik nie został znaleziony");

        if (!userResult.Value.IsActive)
            return Error.Failure("User.Inactive", "Konto użytkownika jest nieaktywne");

        return authResult.Value;
    }

    public async Task<ErrorOr<AuthResult>> RefreshTokenAsync(string refreshToken)
    {
        var authResult = await _authorizationProvider.RefreshTokensAsync(refreshToken);
        if (authResult.IsError)
            return authResult.Errors;

        return authResult.Value;
    }

    public async Task<ErrorOr<Success>> LogoutAsync(string refreshToken)
    {
        var result = await _authorizationProvider.LogoutAsync(refreshToken);
        if (result.IsError)
            return result.Errors;

        return Result.Success;
    }
}
