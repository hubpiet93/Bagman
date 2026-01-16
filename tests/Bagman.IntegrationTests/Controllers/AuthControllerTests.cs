using System.Net;
using System.Text;
using System.Text.Json;
using Bagman.Contracts.Models.Auth;
using Bagman.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using VerifyXunit;
using Xunit;

namespace Bagman.IntegrationTests.Controllers;

[CollectionDefinition("Auth Tests")]
public class AuthTestsCollection : ICollectionFixture<TestFixtures.PostgresFixture>
{
}

/// <summary>
/// Integration tests for AuthController actions using TestContainers for PostgreSQL.
/// Tests Register, Login, Refresh Token, and Logout endpoints with snapshot testing.
/// Uses collection fixture to share one PostgreSQL container across all tests in the class.
/// </summary>
[Collection("Auth Tests")]
public class AuthControllerTests : IAsyncLifetime
{
    private readonly TestFixtures.PostgresFixture _postgresFixture;
    private TestFixtures.AuthTestWebApplicationFactory? _factory;
    private HttpClient? _httpClient;
    private bool _initialized = false;

    public AuthControllerTests(TestFixtures.PostgresFixture postgresFixture)
    {
        // Use injected fixture shared across all tests
        _postgresFixture = postgresFixture;
    }

    public async Task InitializeAsync()
    {
        // Initialize only once
        if (_initialized)
            return;

        _initialized = true;

        // Initialize PostgreSQL container if not already done
        if (_postgresFixture.ConnectionString == null)
        {
            await _postgresFixture.InitializeAsync();
        }

        // Create factory with test connection string
        _factory = new TestFixtures.AuthTestWebApplicationFactory(_postgresFixture.ConnectionString!);
        _httpClient = _factory.CreateClient();

        // Ensure database is created and migrations are applied
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Retry logic for database creation
        int maxRetries = 3;
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                await dbContext.Database.EnsureCreatedAsync();
                break;
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                await Task.Delay(1000);
            }
        }
    }

    public async Task DisposeAsync()
    {
        _httpClient?.Dispose();
        _factory?.Dispose();
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
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _httpClient!.PostAsync("/api/auth/register", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        await Verify(new
        {
            StatusCode = response.StatusCode,
            User = new
            {
                authResponse!.User.Login,
                authResponse.User.Email,
                authResponse.User.IsActive
            },
            HasAccessToken = !string.IsNullOrEmpty(authResponse.AccessToken),
            HasRefreshToken = !string.IsNullOrEmpty(authResponse.RefreshToken),
            ExpiresAtIsInFuture = authResponse.ExpiresAt > DateTime.UtcNow
        });
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
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Register first user
        await _httpClient!.PostAsync("/api/auth/register", content);

        // Try to register with same login
        var request2 = new RegisterRequest
        {
            Login = "duplicateuser",
            Password = "Different@12345",
            Email = "duplicate2@example.com"
        };

        var content2 = new StringContent(
            JsonSerializer.Serialize(request2),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _httpClient.PostAsync("/api/auth/register", content2);

        // Assert
        await Verify(new
        {
            response.StatusCode,
            IsBadRequest = response.StatusCode == HttpStatusCode.BadRequest
        });
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
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _httpClient!.PostAsync("/api/auth/register", content);

        // Assert
        await Verify(new
        {
            response.StatusCode,
            IsBadRequest = response.StatusCode == HttpStatusCode.BadRequest
        });
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
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _httpClient!.PostAsync("/api/auth/register", content);

        // Assert
        await Verify(new
        {
            response.StatusCode,
            IsBadRequest = response.StatusCode == HttpStatusCode.BadRequest
        });
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
            JsonSerializer.Serialize(registerRequest),
            Encoding.UTF8,
            "application/json");

        await _httpClient!.PostAsync("/api/auth/register", registerContent);

        // Act - Login
        var loginRequest = new LoginRequest
        {
            Login = "loginuser",
            Password = "Test@12345"
        };

        var loginContent = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync("/api/auth/login", loginContent);
        var responseBody = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        await Verify(new
        {
            StatusCode = response.StatusCode,
            User = new
            {
                authResponse!.User.Login
            },
            HasAccessToken = !string.IsNullOrEmpty(authResponse.AccessToken),
            HasRefreshToken = !string.IsNullOrEmpty(authResponse.RefreshToken)
        });
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
            JsonSerializer.Serialize(registerRequest),
            Encoding.UTF8,
            "application/json");

        await _httpClient!.PostAsync("/api/auth/register", registerContent);

        // Act - Login with wrong password
        var loginRequest = new LoginRequest
        {
            Login = "passwordtest",
            Password = "WrongPassword@12345"
        };

        var loginContent = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync("/api/auth/login", loginContent);

        // Assert
        await Verify(new
        {
            response.StatusCode,
            IsBadRequest = response.StatusCode == HttpStatusCode.BadRequest
        });
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
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _httpClient!.PostAsync("/api/auth/login", content);

        // Assert
        await Verify(new
        {
            response.StatusCode,
            IsBadRequest = response.StatusCode == HttpStatusCode.BadRequest
        });
    }

    [Fact]
    public async Task Refresh_WithValidRefreshToken_ReturnsOkWithNewTokens()
    {
        // Arrange - Register and Login
        var registerRequest = new RegisterRequest
        {
            Login = "refreshuser",
            Password = "Test@12345",
            Email = "refresh@example.com"
        };

        var registerContent = new StringContent(
            JsonSerializer.Serialize(registerRequest),
            Encoding.UTF8,
            "application/json");

        var registerResponse = await _httpClient!.PostAsync("/api/auth/register", registerContent);
        var registerBody = await registerResponse.Content.ReadAsStringAsync();
        var initialAuthResponse = JsonSerializer.Deserialize<AuthResponse>(
            registerBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var initialAccessToken = initialAuthResponse!.AccessToken;
        var refreshToken = initialAuthResponse.RefreshToken;

        // Act - Refresh token
        var refreshRequest = new RefreshRequest
        {
            RefreshToken = refreshToken
        };

        var refreshContent = new StringContent(
            JsonSerializer.Serialize(refreshRequest),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync("/api/auth/refresh", refreshContent);
        var responseBody = await response.Content.ReadAsStringAsync();
        var newAuthResponse = JsonSerializer.Deserialize<AuthResponse>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        await Verify(new
        {
            StatusCode = response.StatusCode,
            User = new
            {
                newAuthResponse!.User.Login
            },
            HasNewAccessToken = !string.IsNullOrEmpty(newAuthResponse.AccessToken),
            AccessTokenChanged = newAuthResponse.AccessToken != initialAccessToken
        });
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
            JsonSerializer.Serialize(refreshRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _httpClient!.PostAsync("/api/auth/refresh", content);

        // Assert
        await Verify(new
        {
            response.StatusCode,
            IsBadRequest = response.StatusCode == HttpStatusCode.BadRequest
        });
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
            JsonSerializer.Serialize(registerRequest),
            Encoding.UTF8,
            "application/json");

        var registerResponse = await _httpClient!.PostAsync("/api/auth/register", registerContent);
        var registerBody = await registerResponse.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(
            registerBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var refreshToken = authResponse!.RefreshToken;

        // Act - Logout
        var logoutRequest = new LogoutRequest
        {
            RefreshToken = refreshToken
        };

        var logoutContent = new StringContent(
            JsonSerializer.Serialize(logoutRequest),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync("/api/auth/logout", logoutContent);

        // Assert
        await Verify(new
        {
            response.StatusCode,
            IsOk = response.StatusCode == HttpStatusCode.OK
        });
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
            JsonSerializer.Serialize(logoutRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _httpClient!.PostAsync("/api/auth/logout", content);

        // Assert
        await Verify(new
        {
            response.StatusCode,
            IsBadRequest = response.StatusCode == HttpStatusCode.BadRequest
        });
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
            JsonSerializer.Serialize(registerRequest),
            Encoding.UTF8,
            "application/json");

        // Act & Assert - Register
        var registerResponse = await _httpClient!.PostAsync("/api/auth/register", registerContent);
        var registerBody = await registerResponse.Content.ReadAsStringAsync();
        var initialAuth = JsonSerializer.Deserialize<AuthResponse>(
            registerBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var initialAccessToken = initialAuth!.AccessToken;
        var initialRefreshToken = initialAuth.RefreshToken;

        // Act & Assert - Login
        var loginRequest = new LoginRequest
        {
            Login = "fullflowuser",
            Password = "Test@12345"
        };

        var loginContent = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        var loginResponse = await _httpClient.PostAsync("/api/auth/login", loginContent);

        // Act & Assert - Refresh with initial token
        var refreshRequest = new RefreshRequest
        {
            RefreshToken = initialRefreshToken
        };

        var refreshContent = new StringContent(
            JsonSerializer.Serialize(refreshRequest),
            Encoding.UTF8,
            "application/json");

        var refreshResponse = await _httpClient.PostAsync("/api/auth/refresh", refreshContent);
        var refreshBody = await refreshResponse.Content.ReadAsStringAsync();
        var refreshedAuth = JsonSerializer.Deserialize<AuthResponse>(
            refreshBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var newAccessToken = refreshedAuth!.AccessToken;
        var newRefreshToken = refreshedAuth.RefreshToken;

        // Act & Assert - Logout with the NEW refresh token from refresh operation
        var logoutRequest = new LogoutRequest
        {
            RefreshToken = newRefreshToken
        };

        var logoutContent = new StringContent(
            JsonSerializer.Serialize(logoutRequest),
            Encoding.UTF8,
            "application/json");

        var logoutResponse = await _httpClient.PostAsync("/api/auth/logout", logoutContent);

        // Assert complete flow
        await Verify(new
        {
            RegisterStatusCode = registerResponse.StatusCode,
            LoginStatusCode = loginResponse.StatusCode,
            RefreshStatusCode = refreshResponse.StatusCode,
            LogoutStatusCode = logoutResponse.StatusCode,
            RegisterSucceeded = registerResponse.StatusCode == HttpStatusCode.OK,
            LoginSucceeded = loginResponse.StatusCode == HttpStatusCode.OK,
            RefreshSucceeded = refreshResponse.StatusCode == HttpStatusCode.OK,
            LogoutSucceeded = logoutResponse.StatusCode == HttpStatusCode.OK,
            AccessTokenRefreshed = newAccessToken != initialAccessToken,
            RefreshTokenRefreshed = newRefreshToken != initialRefreshToken
        });
    }
}
