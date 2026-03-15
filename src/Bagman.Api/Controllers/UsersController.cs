using System.Security.Claims;
using Bagman.Application.Common;
using Bagman.Application.Features.Users.GetCurrentUser;
using Bagman.Contracts.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Bagman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : AppControllerBase
{
    private readonly FeatureDispatcher _dispatcher;

    public UsersController(FeatureDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    ///     Pobranie profilu aktualnie zalogowanego użytkownika
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var result = await _dispatcher.HandleAsync<GetCurrentUserQuery, CurrentUserResult>(
            new GetCurrentUserQuery { UserId = userId.Value });

        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(new UserResponse
        {
            Id = result.Value.Id,
            Login = result.Value.Login,
            Email = result.Value.Email,
            CreatedAt = result.Value.CreatedAt,
            IsActive = result.Value.IsActive,
            IsSuperAdmin = result.Value.IsSuperAdmin
        });
    }

    private Guid? GetUserId()
    {
        var subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (Guid.TryParse(subClaim, out var userIdFromSub))
            return userIdFromSub;

        var nameIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(nameIdClaim, out var userIdFromNameId))
            return userIdFromNameId;

        return null;
    }
}
