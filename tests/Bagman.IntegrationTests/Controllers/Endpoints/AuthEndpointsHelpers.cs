using Bagman.Contracts.Models.Auth;
using Bagman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bagman.IntegrationTests.Controllers.Endpoints;

/// <summary>
///     Helper methods for Auth-related endpoint testing.
///     Provides utilities for user registration, authentication, and super admin operations.
///     Each method is generic and can return HttpResponseMessage (for snapshot testing)
///     or a concrete type (for deserialization).
/// </summary>
public static class AuthEndpointsHelpers
{
    private const string SuperAdminLogin = "super_admin_test";
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
    ///     Registers a new user.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or AuthResponse for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="request">The registration request.</param>
    /// <param name="token">Optional Bearer token (typically null for registration).</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> RegisterAsync<T>(
        this HttpClient client,
        RegisterRequest request,
        string? token = null) where T : class
    {
        return client.PostAsync<T>("/api/auth/register", request, token);
    }

    /// <summary>
    ///     Logs in an existing user.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or AuthResponse for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="request">The login request.</param>
    /// <param name="token">Optional Bearer token (typically null for login).</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> LoginAsync<T>(
        this HttpClient client,
        LoginRequest request,
        string? token = null) where T : class
    {
        return client.PostAsync<T>("/api/auth/login", request, token);
    }

    #region Request Factory Methods

    /// <summary>
    ///     Creates a RegisterRequest with sensible defaults.
    /// </summary>
    /// <param name="loginPrefix">Prefix for generating a unique login.</param>
    /// <param name="password">The password for the new user.</param>
    /// <param name="emailPrefix">Prefix for generating the email (default: "test").</param>
    /// <returns>A RegisterRequest with a unique login and email.</returns>
    public static RegisterRequest CreateRegisterRequest(
        string loginPrefix,
        string password,
        string emailPrefix = "test")
    {
        var login = Unique(loginPrefix);
        return new RegisterRequest
        {
            Login = login,
            Password = password,
            Email = $"{emailPrefix}+{login}@example.com"
        };
    }

    /// <summary>
    ///     Creates a LoginRequest.
    /// </summary>
    /// <param name="login">The user login.</param>
    /// <param name="password">The user password.</param>
    /// <returns>A LoginRequest.</returns>
    public static LoginRequest CreateLoginRequest(string login, string password)
    {
        return new LoginRequest { Login = login, Password = password };
    }

    #endregion

    #region Scenario Helpers

    /// <summary>
    ///     Registers a new user and returns their authentication token, user ID, and login.
    ///     This is a convenience multi-step scenario helper.
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
        var request = CreateRegisterRequest(loginPrefix, password, emailPrefix);
        var authResponse = await client.RegisterAsync<AuthResponse>(request);
        return (authResponse.AccessToken, authResponse.User.Id, request.Login);
    }

    /// <summary>
    ///     Gets a super admin token for testing admin endpoints.
    ///     Creates a super admin user if one doesn't already exist, then authenticates.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="services">The service provider for database access.</param>
    /// <returns>The bearer token for the super admin user.</returns>
    public static async Task<string> GetSuperAdminTokenAsync(this HttpClient client, IServiceProvider services)
    {
        var registerRequest = new RegisterRequest
        {
            Login = SuperAdminLogin,
            Password = SuperAdminPassword,
            Email = "superadmin@example.com"
        };

        try
        {
            await client.RegisterAsync<AuthResponse>(registerRequest);
        }
        catch
        {
            // User may already exist, that's fine
        }

        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.ExecuteSqlRawAsync(
            $"UPDATE users SET is_super_admin = TRUE WHERE login = '{SuperAdminLogin}';");

        var loginRequest = CreateLoginRequest(SuperAdminLogin, SuperAdminPassword);
        var authResponse = await client.LoginAsync<AuthResponse>(loginRequest);
        return authResponse.AccessToken;
    }

    #endregion
}
