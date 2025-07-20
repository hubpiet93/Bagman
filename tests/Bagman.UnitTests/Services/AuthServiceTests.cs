using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using Bagman.Domain.Services;
using ErrorOr;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Bagman.UnitTests.Services;

[TestFixture]
public class AuthServiceTests
{
    private AuthService _authService = null!;
    private ISupabaseService _supabaseService = null!;
    private IUserRepository _userRepository = null!;

    [SetUp]
    public void Setup()
    {
        _supabaseService = Substitute.For<ISupabaseService>();
        _userRepository = Substitute.For<IUserRepository>();
        _authService = new AuthService(_supabaseService, _userRepository);
    }

    [TestFixture]
    public class RegisterAsyncTests : AuthServiceTests
    {
        [Test]
        public async Task RegisterAsync_WhenUserDoesNotExist_ShouldRegisterSuccessfully()
        {
            // Arrange
            var login = "testuser";
            var password = "password123";
            var email = "test@example.com";
            var expectedUser = new User { Id = Guid.NewGuid(), Login = login, Email = email };
            var expectedAuthResult = new AuthResult
            {
                AccessToken = "access_token",
                RefreshToken = "refresh_token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = expectedUser
            };

            _userRepository.GetByLoginAsync(login).Returns((User?)null);
            _userRepository.GetByEmailAsync(email).Returns((User?)null);
            _supabaseService.RegisterAsync(login, password, email).Returns(expectedAuthResult);

            // Act
            var result = await _authService.RegisterAsync(login, password, email);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(expectedAuthResult);
            await _userRepository.Received(1).GetByLoginAsync(login);
            await _userRepository.Received(1).GetByEmailAsync(email);
            await _supabaseService.Received(1).RegisterAsync(login, password, email);
        }

        [Test]
        public async Task RegisterAsync_WhenUserWithLoginExists_ShouldReturnConflictError()
        {
            // Arrange
            var login = "existinguser";
            var password = "password123";
            var email = "test@example.com";
            var existingUser = new User { Id = Guid.NewGuid(), Login = login, Email = "existing@example.com" };

            _userRepository.GetByLoginAsync(login).Returns(existingUser);

            // Act
            var result = await _authService.RegisterAsync(login, password, email);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().ContainSingle();
            result.Errors[0].Code.Should().Be("User.AlreadyExists");
            result.Errors[0].Description.Should().Be("Użytkownik o podanym loginie już istnieje");
            await _userRepository.Received(1).GetByLoginAsync(login);
            await _userRepository.DidNotReceive().GetByEmailAsync(Arg.Any<string>());
            await _supabaseService.DidNotReceive().RegisterAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public async Task RegisterAsync_WhenUserWithEmailExists_ShouldReturnConflictError()
        {
            // Arrange
            var login = "newuser";
            var password = "password123";
            var email = "existing@example.com";
            var existingUser = new User { Id = Guid.NewGuid(), Login = "differentuser", Email = email };

            _userRepository.GetByLoginAsync(login).Returns((User?)null);
            _userRepository.GetByEmailAsync(email).Returns(existingUser);

            // Act
            var result = await _authService.RegisterAsync(login, password, email);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().ContainSingle();
            result.Errors[0].Code.Should().Be("User.EmailAlreadyExists");
            result.Errors[0].Description.Should().Be("Użytkownik o podanym emailu już istnieje");
            await _userRepository.Received(1).GetByLoginAsync(login);
            await _userRepository.Received(1).GetByEmailAsync(email);
            await _supabaseService.DidNotReceive().RegisterAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public async Task RegisterAsync_WhenGetByLoginFails_ShouldReturnError()
        {
            // Arrange
            var login = "testuser";
            var password = "password123";
            var email = "test@example.com";
            var expectedError = Error.Failure("Database.Error", "Database connection failed");

            _userRepository.GetByLoginAsync(login).Returns(expectedError);

            // Act
            var result = await _authService.RegisterAsync(login, password, email);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().Contain(expectedError);
            await _userRepository.Received(1).GetByLoginAsync(login);
            await _userRepository.DidNotReceive().GetByEmailAsync(Arg.Any<string>());
            await _supabaseService.DidNotReceive().RegisterAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public async Task RegisterAsync_WhenGetByEmailFails_ShouldReturnError()
        {
            // Arrange
            var login = "testuser";
            var password = "password123";
            var email = "test@example.com";
            var expectedError = Error.Failure("Database.Error", "Database connection failed");

            _userRepository.GetByLoginAsync(login).Returns((User?)null);
            _userRepository.GetByEmailAsync(email).Returns(expectedError);

            // Act
            var result = await _authService.RegisterAsync(login, password, email);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().Contain(expectedError);
            await _userRepository.Received(1).GetByLoginAsync(login);
            await _userRepository.Received(1).GetByEmailAsync(email);
            await _supabaseService.DidNotReceive().RegisterAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public async Task RegisterAsync_WhenSupabaseRegistrationFails_ShouldReturnError()
        {
            // Arrange
            var login = "testuser";
            var password = "password123";
            var email = "test@example.com";
            var expectedError = Error.Failure("Supabase.Error", "Registration failed");

            _userRepository.GetByLoginAsync(login).Returns((User?)null);
            _userRepository.GetByEmailAsync(email).Returns((User?)null);
            _supabaseService.RegisterAsync(login, password, email).Returns(expectedError);

            // Act
            var result = await _authService.RegisterAsync(login, password, email);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().Contain(expectedError);
            await _userRepository.Received(1).GetByLoginAsync(login);
            await _userRepository.Received(1).GetByEmailAsync(email);
            await _supabaseService.Received(1).RegisterAsync(login, password, email);
        }
    }

    [TestFixture]
    public class LoginAsyncTests : AuthServiceTests
    {
        [Test]
        public async Task LoginAsync_WhenValidCredentialsAndActiveUser_ShouldLoginSuccessfully()
        {
            // Arrange
            var login = "testuser";
            var password = "password123";
            var expectedUser = new User { Id = Guid.NewGuid(), Login = login, Email = "test@example.com", IsActive = true };
            var expectedAuthResult = new AuthResult
            {
                AccessToken = "access_token",
                RefreshToken = "refresh_token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = expectedUser
            };

            _supabaseService.LoginAsync(login, password).Returns(expectedAuthResult);
            _userRepository.GetByIdAsync(expectedUser.Id).Returns(expectedUser);

            // Act
            var result = await _authService.LoginAsync(login, password);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(expectedAuthResult);
            await _supabaseService.Received(1).LoginAsync(login, password);
            await _userRepository.Received(1).GetByIdAsync(expectedUser.Id);
        }

        [Test]
        public async Task LoginAsync_WhenSupabaseLoginFails_ShouldReturnError()
        {
            // Arrange
            var login = "testuser";
            var password = "wrongpassword";
            var expectedError = Error.Failure("Auth.InvalidCredentials", "Invalid credentials");

            _supabaseService.LoginAsync(login, password).Returns(expectedError);

            // Act
            var result = await _authService.LoginAsync(login, password);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().Contain(expectedError);
            await _supabaseService.Received(1).LoginAsync(login, password);
            await _userRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>());
        }

        [Test]
        public async Task LoginAsync_WhenUserNotFound_ShouldReturnNotFoundError()
        {
            // Arrange
            var login = "testuser";
            var password = "password123";
            var userId = Guid.NewGuid();
            var expectedAuthResult = new AuthResult
            {
                AccessToken = "access_token",
                RefreshToken = "refresh_token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new User { Id = userId, Login = login, Email = "test@example.com" }
            };

            _supabaseService.LoginAsync(login, password).Returns(expectedAuthResult);
            _userRepository.GetByIdAsync(userId).Returns((User?)null);

            // Act
            var result = await _authService.LoginAsync(login, password);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().ContainSingle();
            result.Errors[0].Code.Should().Be("User.NotFound");
            result.Errors[0].Description.Should().Be("Użytkownik nie został znaleziony");
            await _supabaseService.Received(1).LoginAsync(login, password);
            await _userRepository.Received(1).GetByIdAsync(userId);
        }

        [Test]
        public async Task LoginAsync_WhenUserIsInactive_ShouldReturnInactiveError()
        {
            // Arrange
            var login = "testuser";
            var password = "password123";
            var userId = Guid.NewGuid();
            var inactiveUser = new User { Id = userId, Login = login, Email = "test@example.com", IsActive = false };
            var expectedAuthResult = new AuthResult
            {
                AccessToken = "access_token",
                RefreshToken = "refresh_token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new User { Id = userId, Login = login, Email = "test@example.com" }
            };

            _supabaseService.LoginAsync(login, password).Returns(expectedAuthResult);
            _userRepository.GetByIdAsync(userId).Returns(inactiveUser);

            // Act
            var result = await _authService.LoginAsync(login, password);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().ContainSingle();
            result.Errors[0].Code.Should().Be("User.Inactive");
            result.Errors[0].Description.Should().Be("Konto użytkownika jest nieaktywne");
            await _supabaseService.Received(1).LoginAsync(login, password);
            await _userRepository.Received(1).GetByIdAsync(userId);
        }

        [Test]
        public async Task LoginAsync_WhenGetUserByIdFails_ShouldReturnError()
        {
            // Arrange
            var login = "testuser";
            var password = "password123";
            var userId = Guid.NewGuid();
            var expectedAuthResult = new AuthResult
            {
                AccessToken = "access_token",
                RefreshToken = "refresh_token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new User { Id = userId, Login = login, Email = "test@example.com" }
            };
            var expectedError = Error.Failure("Database.Error", "Database connection failed");

            _supabaseService.LoginAsync(login, password).Returns(expectedAuthResult);
            _userRepository.GetByIdAsync(userId).Returns(expectedError);

            // Act
            var result = await _authService.LoginAsync(login, password);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().Contain(expectedError);
            await _supabaseService.Received(1).LoginAsync(login, password);
            await _userRepository.Received(1).GetByIdAsync(userId);
        }
    }

    [TestFixture]
    public class RefreshTokenAsyncTests : AuthServiceTests
    {
        [Test]
        public async Task RefreshTokenAsync_WhenValidRefreshToken_ShouldRefreshSuccessfully()
        {
            // Arrange
            var refreshToken = "valid_refresh_token";
            var expectedAuthResult = new AuthResult
            {
                AccessToken = "new_access_token",
                RefreshToken = "new_refresh_token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new User { Id = Guid.NewGuid(), Login = "testuser", Email = "test@example.com" }
            };

            _supabaseService.RefreshTokenAsync(refreshToken).Returns(expectedAuthResult);

            // Act
            var result = await _authService.RefreshTokenAsync(refreshToken);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(expectedAuthResult);
            await _supabaseService.Received(1).RefreshTokenAsync(refreshToken);
        }

        [Test]
        public async Task RefreshTokenAsync_WhenInvalidRefreshToken_ShouldReturnError()
        {
            // Arrange
            var refreshToken = "invalid_refresh_token";
            var expectedError = Error.Failure("Auth.InvalidRefreshToken", "Invalid refresh token");

            _supabaseService.RefreshTokenAsync(refreshToken).Returns(expectedError);

            // Act
            var result = await _authService.RefreshTokenAsync(refreshToken);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().Contain(expectedError);
            await _supabaseService.Received(1).RefreshTokenAsync(refreshToken);
        }
    }

    [TestFixture]
    public class LogoutAsyncTests : AuthServiceTests
    {
        [Test]
        public async Task LogoutAsync_WhenValidRefreshToken_ShouldLogoutSuccessfully()
        {
            // Arrange
            var refreshToken = "valid_refresh_token";

            _supabaseService.LogoutAsync(refreshToken).Returns<ErrorOr<Success>>(new Success());

            // Act
            var result = await _authService.LogoutAsync(refreshToken);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().Be(Result.Success);
            await _supabaseService.Received(1).LogoutAsync(refreshToken);
        }

        [Test]
        public async Task LogoutAsync_WhenInvalidRefreshToken_ShouldReturnError()
        {
            // Arrange
            var refreshToken = "invalid_refresh_token";
            var expectedError = Error.Failure("Auth.InvalidRefreshToken", "Invalid refresh token");

            _supabaseService.LogoutAsync(refreshToken).Returns(expectedError);

            // Act
            var result = await _authService.LogoutAsync(refreshToken);

            // Assert
            result.IsError.Should().BeTrue();
            result.Errors.Should().Contain(expectedError);
            await _supabaseService.Received(1).LogoutAsync(refreshToken);
        }
    }
} 