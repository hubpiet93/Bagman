using Bagman.Api.Controllers;
using Bagman.Contracts.Models.Auth;
using Bagman.Domain.Models;
using Bagman.Domain.Services;
using ErrorOr;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace Bagman.UnitTests.Controllers;

[TestFixture]
public class AuthControllerTests
{
    private AuthController _controller = null!;
    private IAuthService _authService = null!;

    [SetUp]
    public void Setup()
    {
        _authService = Substitute.For<IAuthService>();
        _controller = new AuthController(_authService);
    }

    [TestFixture]
    public class RegisterTests : AuthControllerTests
    {
        [Test]
        public async Task Register_WhenSuccessful_ShouldReturnOkWithAuthResponse()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "testuser",
                Password = "ValidPassword123!",
                Email = "test@example.com"
            };

            var expectedUser = new User
            {
                Id = Guid.NewGuid(),
                Login = "testuser",
                Email = "test@example.com",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var expectedAuthResult = new AuthResult
            {
                AccessToken = "access_token",
                RefreshToken = "refresh_token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = expectedUser
            };

            _authService.RegisterAsync(request.Login, request.Password, request.Email)
                .Returns(expectedAuthResult);

            // Act
            var result = await _controller.Register(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeOfType<AuthResponse>();

            var authResponse = okResult.Value as AuthResponse;
            authResponse!.AccessToken.Should().Be(expectedAuthResult.AccessToken);
            authResponse.RefreshToken.Should().Be(expectedAuthResult.RefreshToken);
            authResponse.ExpiresAt.Should().Be(expectedAuthResult.ExpiresAt);
            authResponse.User.Id.Should().Be(expectedUser.Id);
            authResponse.User.Login.Should().Be(expectedUser.Login);
            authResponse.User.Email.Should().Be(expectedUser.Email);
            authResponse.User.CreatedAt.Should().Be(expectedUser.CreatedAt);
            authResponse.User.IsActive.Should().Be(expectedUser.IsActive);

            await _authService.Received(1).RegisterAsync(request.Login, request.Password, request.Email);
        }

        [Test]
        public async Task Register_WhenFails_ShouldReturnBadRequestWithErrors()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "testuser",
                Password = "ValidPassword123!",
                Email = "test@example.com"
            };

            var expectedErrors = new List<Error>
            {
                Error.Conflict("User.AlreadyExists", "Użytkownik o podanym loginie już istnieje")
            };

            _authService.RegisterAsync(request.Login, request.Password, request.Email)
                .Returns(Error.Conflict("User.AlreadyExists", "Użytkownik o podanym loginie już istnieje"));

            // Act
            var result = await _controller.Register(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().NotBeNull();
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(System.Text.Json.JsonSerializer.Serialize(badRequestResult.Value));
            dict.Should().ContainKey("errors");
            var errors = dict["errors"] as System.Text.Json.JsonElement?;
            errors.HasValue.Should().BeTrue();
            var errorList = errors.Value.EnumerateArray().Select(e => e.GetString()).ToList();
            errorList.Should().Contain("Użytkownik o podanym loginie już istnieje");

            await _authService.Received(1).RegisterAsync(request.Login, request.Password, request.Email);
        }
    }

    [TestFixture]
    public class LoginTests : AuthControllerTests
    {
        [Test]
        public async Task Login_WhenSuccessful_ShouldReturnOkWithAuthResponse()
        {
            // Arrange
            var request = new LoginRequest
            {
                Login = "testuser",
                Password = "ValidPassword123!"
            };

            var expectedUser = new User
            {
                Id = Guid.NewGuid(),
                Login = "testuser",
                Email = "test@example.com",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var expectedAuthResult = new AuthResult
            {
                AccessToken = "access_token",
                RefreshToken = "refresh_token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = expectedUser
            };

            _authService.LoginAsync(request.Login, request.Password)
                .Returns(expectedAuthResult);

            // Act
            var result = await _controller.Login(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeOfType<AuthResponse>();

            var authResponse = okResult.Value as AuthResponse;
            authResponse!.AccessToken.Should().Be(expectedAuthResult.AccessToken);
            authResponse.RefreshToken.Should().Be(expectedAuthResult.RefreshToken);
            authResponse.ExpiresAt.Should().Be(expectedAuthResult.ExpiresAt);
            authResponse.User.Id.Should().Be(expectedUser.Id);
            authResponse.User.Login.Should().Be(expectedUser.Login);
            authResponse.User.Email.Should().Be(expectedUser.Email);
            authResponse.User.CreatedAt.Should().Be(expectedUser.CreatedAt);
            authResponse.User.IsActive.Should().Be(expectedUser.IsActive);

            await _authService.Received(1).LoginAsync(request.Login, request.Password);
        }

        [Test]
        public async Task Login_WhenFails_ShouldReturnBadRequestWithErrors()
        {
            // Arrange
            var request = new LoginRequest
            {
                Login = "testuser",
                Password = "wrongpassword"
            };

            _authService.LoginAsync(request.Login, request.Password)
                .Returns(Error.Failure("Auth.InvalidCredentials", "Nieprawidłowe dane logowania"));

            // Act
            var result = await _controller.Login(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().NotBeNull();
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(System.Text.Json.JsonSerializer.Serialize(badRequestResult.Value));
            dict.Should().ContainKey("errors");
            var errors = dict["errors"] as System.Text.Json.JsonElement?;
            errors.HasValue.Should().BeTrue();
            var errorList = errors.Value.EnumerateArray().Select(e => e.GetString()).ToList();
            errorList.Should().Contain("Nieprawidłowe dane logowania");

            await _authService.Received(1).LoginAsync(request.Login, request.Password);
        }
    }

    [TestFixture]
    public class RefreshTests : AuthControllerTests
    {
        [Test]
        public async Task Refresh_WhenSuccessful_ShouldReturnOkWithAuthResponse()
        {
            // Arrange
            var request = new RefreshRequest
            {
                RefreshToken = "valid_refresh_token"
            };

            var expectedUser = new User
            {
                Id = Guid.NewGuid(),
                Login = "testuser",
                Email = "test@example.com",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var expectedAuthResult = new AuthResult
            {
                AccessToken = "new_access_token",
                RefreshToken = "new_refresh_token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = expectedUser
            };

            _authService.RefreshTokenAsync(request.RefreshToken)
                .Returns(expectedAuthResult);

            // Act
            var result = await _controller.Refresh(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeOfType<AuthResponse>();

            var authResponse = okResult.Value as AuthResponse;
            authResponse!.AccessToken.Should().Be(expectedAuthResult.AccessToken);
            authResponse.RefreshToken.Should().Be(expectedAuthResult.RefreshToken);
            authResponse.ExpiresAt.Should().Be(expectedAuthResult.ExpiresAt);
            authResponse.User.Id.Should().Be(expectedUser.Id);
            authResponse.User.Login.Should().Be(expectedUser.Login);
            authResponse.User.Email.Should().Be(expectedUser.Email);
            authResponse.User.CreatedAt.Should().Be(expectedUser.CreatedAt);
            authResponse.User.IsActive.Should().Be(expectedUser.IsActive);

            await _authService.Received(1).RefreshTokenAsync(request.RefreshToken);
        }

        [Test]
        public async Task Refresh_WhenFails_ShouldReturnBadRequestWithErrors()
        {
            // Arrange
            var request = new RefreshRequest
            {
                RefreshToken = "invalid_refresh_token"
            };

            var expectedErrors = new List<Error>
            {
                Error.Failure("Auth.InvalidRefreshToken", "Nieprawidłowy refresh token")
            };

            _authService.RefreshTokenAsync(request.RefreshToken)
                .Returns(Error.Failure("Auth.InvalidRefreshToken", "Nieprawidłowy refresh token"));

            // Act
            var result = await _controller.Refresh(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().NotBeNull();
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(System.Text.Json.JsonSerializer.Serialize(badRequestResult.Value));
            dict.Should().ContainKey("errors");
            var errors = dict["errors"] as System.Text.Json.JsonElement?;
            errors.HasValue.Should().BeTrue();
            var errorList = errors.Value.EnumerateArray().Select(e => e.GetString()).ToList();
            errorList.Should().Contain("Nieprawidłowy refresh token");

            await _authService.Received(1).RefreshTokenAsync(request.RefreshToken);
        }
    }

    [TestFixture]
    public class LogoutTests : AuthControllerTests
    {
        [Test]
        public async Task Logout_WhenSuccessful_ShouldReturnOkWithSuccessMessage()
        {
            // Arrange
            var request = new LogoutRequest
            {
                RefreshToken = "valid_refresh_token"
            };

            _authService.LogoutAsync(request.RefreshToken)
                .Returns(new Success());

            // Act
            var result = await _controller.Logout(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(System.Text.Json.JsonSerializer.Serialize(okResult.Value));
            dict.Should().ContainKey("message");
            var message = dict["message"].ToString();
            message.Should().Contain("Wylogowano pomyślnie");

            await _authService.Received(1).LogoutAsync(request.RefreshToken);
        }

        [Test]
        public async Task Logout_WhenFails_ShouldReturnBadRequestWithErrors()
        {
            // Arrange
            var request = new LogoutRequest
            {
                RefreshToken = "invalid_refresh_token"
            };

            _authService.LogoutAsync(request.RefreshToken)
                .Returns(Error.Failure("Auth.InvalidRefreshToken", "Nieprawidłowy refresh token"));

            // Act
            var result = await _controller.Logout(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().NotBeNull();
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(System.Text.Json.JsonSerializer.Serialize(badRequestResult.Value));
            dict.Should().ContainKey("errors");
            var errors = dict["errors"] as System.Text.Json.JsonElement?;
            errors.HasValue.Should().BeTrue();
            var errorList = errors.Value.EnumerateArray().Select(e => e.GetString()).ToList();
            errorList.Should().Contain("Nieprawidłowy refresh token");

            await _authService.Received(1).LogoutAsync(request.RefreshToken);
        }
    }
} 