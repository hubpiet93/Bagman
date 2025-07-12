using TypowanieMeczy.Domain.Interfaces;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace TypowanieMeczy.Infrastructure.Services;

public class SupabaseAuthService : IAuthService
{
    private readonly ISupabaseClient _supabaseClient;
    private readonly ILogger<SupabaseAuthService> _logger;

    public SupabaseAuthService(ISupabaseClient supabaseClient, ILogger<SupabaseAuthService> logger)
    {
        _supabaseClient = supabaseClient;
        _logger = logger;
    }

    public async Task<AuthResult> RegisterAsync(Login login, Email email, string password)
    {
        try
        {
            var response = await _supabaseClient.Auth.SignUpAsync(email.Value, password);

            if (response.User != null)
            {
                return new AuthResult
                {
                    Success = true,
                    UserId = UserId.FromString(response.User.Id.ToString()),
                    AccessToken = response.AccessToken,
                    RefreshToken = response.RefreshToken
                };
            }

            return new AuthResult { Success = false, ErrorMessage = "Registration failed" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return new AuthResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<AuthResult> LoginAsync(Login login, string password)
    {
        try
        {
            // First get user by login
            var user = await GetUserByLoginAsync(login);
            if (user == null)
            {
                return new AuthResult { Success = false, ErrorMessage = "User not found" };
            }

            var response = await _supabaseClient.Auth.SignInAsync(user.Email ?? string.Empty, password);

            if (response.User != null)
            {
                return new AuthResult
                {
                    Success = true,
                    UserId = UserId.FromString(response.User.Id.ToString()),
                    AccessToken = response.AccessToken,
                    RefreshToken = response.RefreshToken
                };
            }

            return new AuthResult { Success = false, ErrorMessage = "Login failed" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return new AuthResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<bool> LogoutAsync(string accessToken)
    {
        try
        {
            await _supabaseClient.Auth.SignOutAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return false;
        }
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        try
        {
            var supabaseUser = await _supabaseClient.Auth.GetUserAsync();
            if (supabaseUser == null)
            {
                return null;
            }

            // Get user data from Supabase
            var response = await _supabaseClient
                .From<SupabaseUser>()
                .Select("*")
                .Filter("id", Postgrest.Constants.Operator.Equals, supabaseUser.Id.ToString())
                .GetAsync();

            var userData = response.FirstOrDefault();
            if (userData == null)
            {
                return null;
            }

            // Convert to domain User entity
            var userId = UserId.FromString(userData.Id.ToString());
            var login = new Login(userData.Login);
            var email = new Email(userData.Email ?? string.Empty);
            var passwordHash = new PasswordHash(userData.PasswordHash);

            var user = new User(login, email, passwordHash);
            
            // Set the ID manually since it's already generated
            var idProperty = typeof(User).GetProperty("Id");
            idProperty?.SetValue(user, userId);

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return null;
        }
    }

    private async Task<SupabaseUser?> GetUserByLoginAsync(Login login)
    {
        try
        {
            var responseList = await _supabaseClient
                .From<SupabaseUser>()
                .Select("*")
                .Filter("login", Postgrest.Constants.Operator.Equals, login.Value)
                .GetAsync();

            return responseList.FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }
}

public class AuthResult
{
    public bool Success { get; set; }
    public UserId? UserId { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? ErrorMessage { get; set; }
} 