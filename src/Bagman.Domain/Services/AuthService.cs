using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using ErrorOr;
using Result = ErrorOr.Result;

namespace Bagman.Domain.Services;

public class AuthService : IAuthService
{
    private readonly ISupabaseService _supabaseService;
    private readonly IUserRepository _userRepository;

    public AuthService(ISupabaseService supabaseService, IUserRepository userRepository)
    {
        _supabaseService = supabaseService;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<AuthResult>> RegisterAsync(string login, string password, string email)
    {
        // Sprawdź czy użytkownik już istnieje
        var existingUserResult = await _userRepository.GetByLoginAsync(login);
        if (existingUserResult.IsError)
        {
            return existingUserResult.Errors;
        }

        if (existingUserResult.Value is not null)
        {
            return Error.Conflict("User.AlreadyExists", "Użytkownik o podanym loginie już istnieje");
        }

        var existingEmailResult = await _userRepository.GetByEmailAsync(email);
        if (existingEmailResult.IsError)
        {
            return existingEmailResult.Errors;
        }

        if (existingEmailResult.Value is not null)
        {
            return Error.Conflict("User.EmailAlreadyExists", "Użytkownik o podanym emailu już istnieje");
        }

        // Rejestracja przez Supabase (to już tworzy użytkownika w tabeli users)
        var authResult = await _supabaseService.RegisterAsync(login, password, email);
        if (authResult.IsError)
        {
            return authResult.Errors;
        }

        return authResult.Value;
    }

    public async Task<ErrorOr<AuthResult>> LoginAsync(string login, string password)
    {
        // Logowanie przez Supabase
        var authResult = await _supabaseService.LoginAsync(login, password);
        if (authResult.IsError)
        {
            return authResult.Errors;
        }

        // Sprawdź czy użytkownik jest aktywny
        var userResult = await _userRepository.GetByIdAsync(authResult.Value.User.Id);
        if (userResult.IsError)
        {
            return userResult.Errors;
        }

        if (userResult.Value is null)
        {
            return Error.NotFound("User.NotFound", "Użytkownik nie został znaleziony");
        }

        if (!userResult.Value.IsActive)
        {
            return Error.Failure("User.Inactive", "Konto użytkownika jest nieaktywne");
        }

        return authResult.Value;
    }

    public async Task<ErrorOr<AuthResult>> RefreshTokenAsync(string refreshToken)
    {
        // Odświeżanie tokenu przez Supabase
        var authResult = await _supabaseService.RefreshTokenAsync(refreshToken);
        if (authResult.IsError)
        {
            return authResult.Errors;
        }

        return authResult.Value;
    }

    public async Task<ErrorOr<Success>> LogoutAsync(string refreshToken)
    {
        // Wylogowanie przez Supabase
        var result = await _supabaseService.LogoutAsync(refreshToken);
        if (result.IsError)
        {
            return result.Errors;
        }

        return Result.Success;
    }
}
