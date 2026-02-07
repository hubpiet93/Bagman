using Bagman.Contracts.Models.Auth;
using Bagman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Bagman.IntegrationTests.Controllers.Endpoints;

/// <summary>
///     Helper methods for Auth-related endpoint testing.
///     Provides utilities for user registration, authentication, and super admin operations.
/// </summary>
public static class AuthEndpointsHelpers
{
    /// <summary>
    ///     The default super admin login used in tests.
    /// </summary>
    private const string SuperAdminLogin = "super_admin_test";

    /// <summary>
    ///     The default super admin password used in tests.
    /// </summary>
    private const string SuperAdminPassword = "SuperAdmin@12345";

    /// <summary>
    ///     Generates a unique string by combining a prefix with a short GUID suffix.
    ///     Useful for creating unique logins and table names.
    /// </summary>
    /// <param name="prefix">The prefix for the unique string.</param>
    /// <returns>A unique string in format "prefix_guidshortened".</returns>
    public static string Unique(string prefix)
    {
        return $"{prefix}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
    }

    /// <summary>
    ///     Registers a new user and returns their authentication token, user ID, and login.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="loginPrefix">Prefix for generating a unique login.</param>
    /// <param name="password">The password for the new user.</param>
    /// <param name="emailPrefix">Prefix for generating the email (default: "test").</param>
    /// <returns>Tuple containing (token, userId, login).</returns>
    public static async Task<(string Token, Guid UserId, string Login)> RegisterAndGetTokenAsync(
        this HttpClient client,
        string loginPrefix,
        string password,
        string emailPrefix = "test")
    {
        var login = Unique(loginPrefix);

        var registerRequest = new RegisterRequest
        {
            Login = login,
            Password = password,
            Email = $"{emailPrefix}+{login}@example.com"
        };

        var authResponse = await client.PostAsJsonAsync<AuthResponse>("/api/auth/register", registerRequest);

        return (authResponse.AccessToken, authResponse.User.Id, login);
    }

    /// <summary>
    ///     Gets a super admin token for testing admin endpoints.
    ///     Creates a super admin user if one doesn't already exist, then authenticates.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="services"></param>
    /// <returns>The bearer token for the super admin user.</returns>
    public static async Task<string> GetSuperAdminTokenAsync(this HttpClient client, IServiceProvider services)
    {
        // Try to register super admin (may fail if already exists, which is ok)
        var registerRequest = new RegisterRequest
        {
            Login = SuperAdminLogin,
            Password = SuperAdminPassword,
            Email = "superadmin@example.com"
        };

        try
        {
            await client.PostAsJsonAsync<AuthResponse>("/api/auth/register", registerRequest);
        }
        catch
        {
            // User may already exist, that's fine
        }
        
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.ExecuteSqlRawAsync(
            $"UPDATE users SET is_super_admin = TRUE WHERE login = '{SuperAdminLogin}';");

        // Login to get token
        var loginRequest = new LoginRequest
        {
            Login = SuperAdminLogin,
            Password = SuperAdminPassword
        };

        var authResponse = await client.PostAsJsonAsync<AuthResponse>("/api/auth/login", loginRequest);
        return authResponse.AccessToken;
    }
}
