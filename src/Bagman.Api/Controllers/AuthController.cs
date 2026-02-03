using Bagman.Api.Controllers.Mappers;
using Bagman.Contracts.Models;
using Bagman.Contracts.Models.Auth;
using Bagman.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bagman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : AppControllerBase
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
            authResult => Ok(authResult.ToAuthResponse()),
            BadRequest
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
            authResult => Ok(authResult.ToAuthResponse()),
            BadRequest
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
            authResult => Ok(authResult.ToAuthResponse()),
            BadRequest
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
            _ => Ok(new SuccessResponse("Wylogowano pomyślnie")),
            BadRequest
        );
    }
}
