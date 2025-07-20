using Bagman.Contracts.Models.Auth;
using Bagman.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bagman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    ///     Rejestracja nowego użytkownika
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request.Login, request.Password, request.Email);

        return result.Match<IActionResult>(
            authResult => Ok(new AuthResponse
            {
                AccessToken = authResult.AccessToken,
                RefreshToken = authResult.RefreshToken,
                ExpiresAt = authResult.ExpiresAt,
                User = new UserResponse
                {
                    Id = authResult.User.Id,
                    Login = authResult.User.Login,
                    Email = authResult.User.Email,
                    CreatedAt = authResult.User.CreatedAt,
                    IsActive = authResult.User.IsActive
                }
            }),
            errors => BadRequest(new {errors = errors.Select(e => e.Description)})
        );
    }

    /// <summary>
    ///     Logowanie użytkownika
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.Login, request.Password);

        return result.Match<IActionResult>(
            authResult => Ok(new AuthResponse
            {
                AccessToken = authResult.AccessToken,
                RefreshToken = authResult.RefreshToken,
                ExpiresAt = authResult.ExpiresAt,
                User = new UserResponse
                {
                    Id = authResult.User.Id,
                    Login = authResult.User.Login,
                    Email = authResult.User.Email,
                    CreatedAt = authResult.User.CreatedAt,
                    IsActive = authResult.User.IsActive
                }
            }),
            errors => BadRequest(new {errors = errors.Select(e => e.Description)})
        );
    }

    /// <summary>
    ///     Odświeżenie tokenu dostępu
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);

        return result.Match<IActionResult>(
            authResult => Ok(new AuthResponse
            {
                AccessToken = authResult.AccessToken,
                RefreshToken = authResult.RefreshToken,
                ExpiresAt = authResult.ExpiresAt,
                User = new UserResponse
                {
                    Id = authResult.User.Id,
                    Login = authResult.User.Login,
                    Email = authResult.User.Email,
                    CreatedAt = authResult.User.CreatedAt,
                    IsActive = authResult.User.IsActive
                }
            }),
            errors => BadRequest(new {errors = errors.Select(e => e.Description)})
        );
    }

    /// <summary>
    ///     Wylogowanie użytkownika
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var result = await _authService.LogoutAsync(request.RefreshToken);

        return result.Match<IActionResult>(
            _ => Ok(new {message = "Wylogowano pomyślnie"}),
            errors => BadRequest(new {errors = errors.Select(e => e.Description)})
        );
    }
}
