using System.Text;
using Bagman.Contracts.Models.Auth;
using Bagman.IntegrationTests.TestFixtures;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace Bagman.IntegrationTests.Controllers;

[CollectionDefinition("Auth Tests")]
public class AuthTestsCollection : ICollectionFixture<PostgresFixture>
{
}

/// <summary>
///     Integration tests for AuthController actions using TestContainers for PostgreSQL.
///     Tests Register, Login, Refresh Token, and Logout endpoints with snapshot testing.
///     Uses collection fixture to share one PostgreSQL container across all tests in the class.
/// </summary>
[Collection("Auth Tests")]
public class AuthControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    public AuthControllerTests(PostgresFixture postgresFixture, ITestOutputHelper testOutputHelper) : base(postgresFixture, testOutputHelper)
    {
    }

    public async Task InitializeAsync()
    {
        await Init();
    }

    public new async Task DisposeAsync()
    {
        await Dispose();
    }

    [Fact]
    public async Task Register_WithValidRequest_ReturnsOkWithAuthResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Login = "testuser",
            Password = "Test@12345",
            Email = "test@example.com"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json");

        // Act
        await HttpClient.PostAsync("/api/auth/register", content);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task Register_WithDuplicateLogin_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Login = "duplicateuser",
            Password = "Test@12345",
            Email = "duplicate1@example.com"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json");

        // Register first user
        await HttpClient.PostAsync("/api/auth/register", content);

        // Try to register with same login
        var request2 = new RegisterRequest
        {
            Login = "duplicateuser",
            Password = "Different@12345",
            Email = "duplicate2@example.com"
        };

        var content2 = new StringContent(
            JsonConvert.SerializeObject(request2),
            Encoding.UTF8,
            "application/json");

        // Act
        await HttpClient.PostAsync("/api/auth/register", content2);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Login = "testuser",
            Password = "Test@12345",
            Email = "invalid-email"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json");

        // Act
        await HttpClient.PostAsync("/api/auth/register", content);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task Register_WithWeakPassword_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Login = "testuser",
            Password = "weak",
            Email = "test@example.com"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json");

        // Act
        await HttpClient.PostAsync("/api/auth/register", content);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithAuthResponse()
    {
        // Arrange - Register first
        var registerRequest = new RegisterRequest
        {
            Login = "loginuser",
            Password = "Test@12345",
            Email = "login@example.com"
        };

        var registerContent = new StringContent(
            JsonConvert.SerializeObject(registerRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/auth/register", registerContent);

        // Act - Login
        var loginRequest = new LoginRequest
        {
            Login = "loginuser",
            Password = "Test@12345"
        };

        var loginContent = new StringContent(
            JsonConvert.SerializeObject(loginRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/auth/login", loginContent);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsBadRequest()
    {
        // Arrange - Register first
        var registerRequest = new RegisterRequest
        {
            Login = "passwordtest",
            Password = "Test@12345",
            Email = "password@example.com"
        };

        var registerContent = new StringContent(
            JsonConvert.SerializeObject(registerRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/auth/register", registerContent);

        // Act - Login with wrong password
        var loginRequest = new LoginRequest
        {
            Login = "passwordtest",
            Password = "WrongPassword@12345"
        };

        var loginContent = new StringContent(
            JsonConvert.SerializeObject(loginRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/auth/login", loginContent);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task Login_WithNonexistentUser_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Login = "nonexistent",
            Password = "Test@12345"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(loginRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        await HttpClient.PostAsync("/api/auth/login", content);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task Refresh_WithValidRefreshToken_ReturnsOkWithNewTokens()
    {
        // Arrange - Register
        var registerRequest = new RegisterRequest
        {
            Login = "refreshuser",
            Password = "Test@12345",
            Email = "refresh@example.com"
        };

        var registerContent = new StringContent(
            JsonConvert.SerializeObject(registerRequest),
            Encoding.UTF8,
            "application/json");

        var registerResponse = await HttpClient.PostAsync("/api/auth/register", registerContent);
        var registerBody = await registerResponse.Content.ReadAsStringAsync();
        var initialAuthResponse = JsonConvert.DeserializeObject<AuthResponse>(registerBody);

        var refreshToken = initialAuthResponse!.RefreshToken;

        // Act - Refresh token
        var refreshRequest = new RefreshRequest
        {
            RefreshToken = refreshToken
        };

        var refreshContent = new StringContent(
            JsonConvert.SerializeObject(refreshRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/auth/refresh", refreshContent);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task Refresh_WithInvalidRefreshToken_ReturnsBadRequest()
    {
        // Arrange
        var refreshRequest = new RefreshRequest
        {
            RefreshToken = "invalid_token_xyz"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(refreshRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        await HttpClient.PostAsync("/api/auth/refresh", content);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task Logout_WithValidRefreshToken_ReturnsOk()
    {
        // Arrange - Register
        var registerRequest = new RegisterRequest
        {
            Login = "logoutuser",
            Password = "Test@12345",
            Email = "logout@example.com"
        };

        var registerContent = new StringContent(
            JsonConvert.SerializeObject(registerRequest),
            Encoding.UTF8,
            "application/json");

        var registerResponse = await HttpClient.PostAsync("/api/auth/register", registerContent);
        var registerBody = await registerResponse.Content.ReadAsStringAsync();
        var authResponse = JsonConvert.DeserializeObject<AuthResponse>(registerBody);

        var refreshToken = authResponse!.RefreshToken;

        // Act - Logout
        var logoutRequest = new LogoutRequest
        {
            RefreshToken = refreshToken
        };

        var logoutContent = new StringContent(
            JsonConvert.SerializeObject(logoutRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/auth/logout", logoutContent);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task Logout_WithInvalidRefreshToken_ReturnsBadRequest()
    {
        // Arrange
        var logoutRequest = new LogoutRequest
        {
            RefreshToken = "invalid_token_xyz"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(logoutRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        await HttpClient.PostAsync("/api/auth/logout", content);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task FullAuthenticationFlow_RegisterLoginRefreshLogout_Succeeds()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Login = "fullflowuser",
            Password = "Test@12345",
            Email = "fullflow@example.com"
        };

        var registerContent = new StringContent(
            JsonConvert.SerializeObject(registerRequest),
            Encoding.UTF8,
            "application/json");

        // Act - Register
        var registerResponse = await HttpClient.PostAsync("/api/auth/register", registerContent);
        var registerBody = await registerResponse.Content.ReadAsStringAsync();
        var initialAuth = JsonConvert.DeserializeObject<AuthResponse>(registerBody);

        var initialRefreshToken = initialAuth!.RefreshToken;

        // Act - Login
        var loginRequest = new LoginRequest
        {
            Login = "fullflowuser",
            Password = "Test@12345"
        };

        var loginContent = new StringContent(
            JsonConvert.SerializeObject(loginRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/auth/login", loginContent);

        // Act - Refresh with initial token
        var refreshRequest = new RefreshRequest
        {
            RefreshToken = initialRefreshToken
        };

        var refreshContent = new StringContent(
            JsonConvert.SerializeObject(refreshRequest),
            Encoding.UTF8,
            "application/json");

        var refreshResponse = await HttpClient.PostAsync("/api/auth/refresh", refreshContent);
        var refreshBody = await refreshResponse.Content.ReadAsStringAsync();
        var refreshedAuth = JsonConvert.DeserializeObject<AuthResponse>(refreshBody);

        var newRefreshToken = refreshedAuth!.RefreshToken;

        // Act - Logout with the NEW refresh token from refresh operation
        var logoutRequest = new LogoutRequest
        {
            RefreshToken = newRefreshToken
        };

        var logoutContent = new StringContent(
            JsonConvert.SerializeObject(logoutRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/auth/logout", logoutContent);

        // Assert (snapshot for the whole flow: register + login + refresh + logout)
        await VerifyHttpRecording();
    }
}
