using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Infrastructure.Services;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(Login login, Email email, string password);
    Task<AuthResult> LoginAsync(Login login, string password);
    Task<bool> LogoutAsync(string accessToken);
    Task<User?> GetCurrentUserAsync();
} 