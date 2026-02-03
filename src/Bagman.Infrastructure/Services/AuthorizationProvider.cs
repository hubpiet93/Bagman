using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using Bagman.Domain.Services;
using ErrorOr;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Bagman.Infrastructure.Services;

public class AuthorizationProvider : IAuthorizationProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthorizationProvider> _logger;
    private readonly IUserRepository _userRepository;

    public AuthorizationProvider(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthorizationProvider> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ErrorOr<AuthResult>> RegisterAsync(string login, string password, string email)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = login,
            Email = email,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var createResult = await _userRepository.CreateAsync(user);
        if (createResult.IsError)
            return createResult.Errors;

        // Store password hash in DB via repository
        var hash = HashPassword(password);
        var setHashResult = await _userRepository.SetPasswordHashAsync(user.Id, hash);
        if (setHashResult.IsError)
            return setHashResult.Errors;

        var tokens = await GenerateTokens(user);
        return tokens;
    }

    public async Task<ErrorOr<AuthResult>> LoginAsync(string login, string password)
    {
        var userResult = await _userRepository.GetByLoginAsync(login);
        if (userResult.IsError)
            return userResult.Errors;

        var user = userResult.Value;
        if (user is null)
            return Error.Unauthorized("Auth.InvalidCredentials", "Nieprawidłowy login lub hasło");

        var storedHashResult = await _userRepository.GetPasswordHashAsync(user.Id);
        if (storedHashResult.IsError)
            return storedHashResult.Errors;

        var storedHash = storedHashResult.Value;
        if (string.IsNullOrEmpty(storedHash))
            return Error.Failure("Auth.NoCredentials", "Dla tego użytkownika nie ma zapisanych danych uwierzytelniających");

        if (!VerifyPassword(storedHash, password))
            return Error.Unauthorized("Auth.InvalidCredentials", "Nieprawidłowy login lub hasło");

        var tokens = await GenerateTokens(user);
        return tokens;
    }

    public async Task<ErrorOr<AuthResult>> RefreshTokensAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Error.Validation("Auth.InvalidRefresh", "Refresh token jest nieprawidłowy");

        var tokenEntryResult = await _userRepository.GetRefreshTokenAsync(refreshToken);
        if (tokenEntryResult.IsError)
            return tokenEntryResult.Errors;

        var tokenEntry = tokenEntryResult.Value;
        if (tokenEntry is null)
            return Error.NotFound("Auth.RefreshNotFound", "Refresh token nie został znaleziony");

        if (tokenEntry.ExpiresAt < DateTime.UtcNow)
        {
            await _userRepository.RemoveRefreshTokenAsync(refreshToken);
            return Error.Validation("Auth.RefreshExpired", "Refresh token wygasł");
        }

        var userResult = await _userRepository.GetByIdAsync(tokenEntry.UserId);
        if (userResult.IsError)
            return userResult.Errors;

        var user = userResult.Value;
        if (user is null)
            return Error.NotFound("User.NotFound", "Użytkownik nie został znaleziony");

        // Invalidate used refresh token
        await _userRepository.RemoveRefreshTokenAsync(refreshToken);

        var tokens = await GenerateTokens(user);
        return tokens;
    }

    public async Task<ErrorOr<Success>> LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Error.Validation("Auth.InvalidRefresh", "Refresh token jest nieprawidłowy");

        // Remove refresh token from persistent store
        var result = await _userRepository.RemoveRefreshTokenAsync(refreshToken);

        // Even if token doesn't exist, consider logout successful (idempotent operation)
        if (result.IsError)
            _logger.LogWarning("Failed to remove refresh token during logout: {Code}",
                result.FirstError.Code);
        // Don't return error - logout is considered successful if token is already gone
        return Result.Success;
    }

    public Task<ErrorOr<bool>> ValidateJwtAsync(string jwt)
    {
        if (string.IsNullOrWhiteSpace(jwt))
            return Task.FromResult<ErrorOr<bool>>(Error.Validation("Auth.InvalidJwt", "JWT jest nieprawidłowy"));

        var secret = _configuration["Jwt:Secret"] ?? "dev-secret-change-me";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(jwt, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return Task.FromResult<ErrorOr<bool>>(true);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "JWT validation failed");
            return Task.FromResult<ErrorOr<bool>>(false);
        }
    }

    private async Task<AuthResult> GenerateTokens(User user)
    {
        var secret = _configuration["Jwt:Secret"] ?? "dev-secret-change-me";
        var accessMinutes = int.TryParse(_configuration["Jwt:AccessTokenExpirationMinutes"], out var m) ? m : 60;
        var refreshDays = int.TryParse(_configuration["Jwt:RefreshTokenExpirationDays"], out var d) ? d : 7;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(accessMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Login),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("IsSuperAdmin", user.IsSuperAdmin.ToString().ToLower())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: creds
        );

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.WriteToken(token);

        // Create refresh token and persist via repository
        var refreshToken = CreateRefreshToken();
        var refreshExpiry = now.AddDays(refreshDays);
        await _userRepository.AddRefreshTokenAsync(new RefreshToken {Token = refreshToken, UserId = user.Id, ExpiresAt = refreshExpiry});

        return new AuthResult
        {
            AccessToken = jwt,
            RefreshToken = refreshToken,
            ExpiresAt = expires,
            User = user
        };
    }

    private static string CreateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    // Simple PBKDF2-based password hashing (for demo). Replace with a
    // permanent, audited implementation and persist the hash in DB for prod.
    private static string HashPassword(string password)
    {
        const int saltSize = 16;
        const int keySize = 32;
        const int iterations = 100_000;

        var salt = RandomNumberGenerator.GetBytes(saltSize);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        var key = pbkdf2.GetBytes(keySize);

        // store as: iterations.salt.key (all base64)
        return string.Join('.', iterations.ToString(), Convert.ToBase64String(salt), Convert.ToBase64String(key));
    }

    private static bool VerifyPassword(string storedHash, string password)
    {
        try
        {
            var parts = storedHash.Split('.', 3);
            if (parts.Length != 3)
                return false;

            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var attempted = pbkdf2.GetBytes(key.Length);

            return CryptographicOperations.FixedTimeEquals(attempted, key);
        }
        catch
        {
            return false;
        }
    }
}
