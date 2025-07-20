using Bagman.Domain.Models;
using Bagman.Domain.Services;
using Bagman.Infrastructure.Models;
using ErrorOr;
using Postgrest;
using Postgrest.Exceptions;
using Supabase.Gotrue;
using Client = Supabase.Client;
using Constants = Postgrest.Constants;
using User = Bagman.Domain.Models.User;

namespace Bagman.Infrastructure.Services;

public class SupabaseService : ISupabaseService
{
    private readonly Client _supabaseClient;

    public SupabaseService(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<ErrorOr<AuthResult>> RegisterAsync(string login, string password, string email)
    {
        try
        {
            // Sprawdź czy użytkownik już istnieje w naszej bazie danych
            var existingUsers = await _supabaseClient
                .From<UserEntity>()
                .Select("*")
                .Filter(x => x.Login, Constants.Operator.Equals, login)
                .Get();

            if (existingUsers.Models.Count > 0)
                return Error.Conflict("Auth.LoginAlreadyExists", "Użytkownik z tym loginem już istnieje");

            var options = new SignUpOptions
            {
                Data = new Dictionary<string, object>
                {
                    {"login", login}
                }
            };

            var response = await _supabaseClient.Auth.SignUp(email, password, options);

            if (response.User is null)
                return Error.Failure("Auth.RegistrationFailed", "Rejestracja nie powiodła się");

            // Ręcznie utwórz użytkownika w tabeli users
            var userEntity = new UserEntity
            {
                Id = Guid.Parse(response.User.Id),
                Login = login,
                Email = email,
                CreatedAt = response.User.CreatedAt,
                IsActive = true
            };

            try
            {
                // Spróbujmy użyć Insert z opcją OnConflict
                await _supabaseClient
                    .From<UserEntity>()
                    .Insert(userEntity, new QueryOptions {OnConflict = "id", Upsert = true});
            }
            catch (PostgrestException ex) when (ex.Message.Contains("duplicate key"))
            {
                // Użytkownik już istnieje, to jest OK
            }
            catch (Exception ex)
            {
                throw;
            }

            // Po rejestracji zwróć dane użytkownika bez tokenów
            // Użytkownik będzie musiał się zalogować osobno
            return new AuthResult
            {
                AccessToken = response.AccessToken ?? string.Empty, // Brak tokenów po rejestracji
                RefreshToken = response.RefreshToken ?? string.Empty,
                ExpiresAt = DateTime.UtcNow.AddHours(1), // Domyślny czas wygaśnięcia
                User = new User
                {
                    Id = Guid.Parse(response.User.Id),
                    Login = login,
                    Email = email,
                    CreatedAt = response.User.CreatedAt,
                    IsActive = true
                }
            };
        }
        catch (Exception ex)
        {
            return Error.Failure("Auth.RegistrationError", $"Błąd podczas tworzenia użytkownika: {ex.Message}");
        }
    }

    public async Task<ErrorOr<AuthResult>> LoginAsync(string login, string password)
    {
        try
        {
            // Najpierw znajdź użytkownika po loginie w naszej bazie danych
            var userResponse = await _supabaseClient
                .From<UserEntity>()
                .Select("*")
                .Filter(x => x.Login, Constants.Operator.Equals, login)
                .Single();

            if (userResponse is null)
                return Error.NotFound("Auth.UserNotFound", "Nieprawidłowy login lub hasło");

            // Teraz zaloguj się przez Supabase używając emaila
            var response = await _supabaseClient.Auth.SignIn(userResponse.Email, password);

            if (response.User is null)
                return Error.NotFound("Auth.UserNotFound", "Nieprawidłowy login lub hasło");

            var session = response;
            if (session is null)
                return Error.Failure("Auth.NoSession", "Nie udało się utworzyć sesji");

            return new AuthResult
            {
                AccessToken = session.AccessToken ?? string.Empty,
                RefreshToken = session.RefreshToken ?? string.Empty,
                ExpiresAt = session.ExpiresAt(),
                User = new User
                {
                    Id = Guid.Parse(response.User.Id),
                    Login = userResponse.Login,
                    Email = response.User.Email ?? string.Empty,
                    CreatedAt = response.User.CreatedAt,
                    IsActive = userResponse.IsActive
                }
            };
        }
        catch (PostgrestException ex) when (ex.Message.Contains("No rows found"))
        {
            return Error.NotFound("Auth.UserNotFound", "Nieprawidłowy login lub hasło");
        }
        catch (Exception ex)
        {
            return Error.Failure("Auth.LoginError", $"Błąd podczas logowania: {ex.Message}");
        }
    }

    public async Task<ErrorOr<AuthResult>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var response = await _supabaseClient.Auth.RefreshSession();

            if (response.User is null)
                return Error.Unauthorized("Auth.InvalidRefreshToken", "Nieprawidłowy refresh token");

            var session = response;
            if (session is null)
                return Error.Failure("Auth.NoSession", "Nie udało się odświeżyć sesji");

            // Pobierz dane użytkownika z bazy danych
            var userResponse = await _supabaseClient
                .From<UserEntity>()
                .Select("*")
                .Filter(x => x.Id, Constants.Operator.Equals, response.User.Id)
                .Single();

            return new AuthResult
            {
                AccessToken = session.AccessToken ?? string.Empty,
                RefreshToken = session.RefreshToken ?? string.Empty,
                ExpiresAt = session.ExpiresAt(),
                User = new User
                {
                    Id = Guid.Parse(response.User.Id),
                    Login = userResponse.Login,
                    Email = response.User.Email ?? string.Empty,
                    CreatedAt = response.User.CreatedAt,
                    IsActive = userResponse.IsActive
                }
            };
        }
        catch (PostgrestException ex) when (ex.Message.Contains("No rows found"))
        {
            return Error.NotFound("Auth.UserNotFound", "Użytkownik nie został znaleziony");
        }
        catch (Exception ex)
        {
            return Error.Failure("Auth.RefreshError", $"Błąd podczas odświeżania tokenu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Success>> LogoutAsync(string refreshToken)
    {
        try
        {
            await _supabaseClient.Auth.SignOut();
            return new Success();
        }
        catch (Exception ex)
        {
            return Error.Failure("Auth.LogoutError", $"Błąd podczas wylogowania: {ex.Message}");
        }
    }
}
