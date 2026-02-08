using System.Security.Claims;
using Bagman.Api.Attributes;
using Bagman.Api.Controllers.Mappers;
using Bagman.Application.Common;
using Bagman.Application.Features.Matches.CreateMatch;
using Bagman.Application.Features.Matches.DeleteMatch;
using Bagman.Application.Features.Matches.SetMatchResult;
using Bagman.Application.Features.Matches.UpdateMatch;
using Bagman.Contracts.Models;
using Bagman.Contracts.Models.Tables;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bagman.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize]
[SuperAdminOnly]
public class AdminMatchesController : AppControllerBase
{
    private readonly FeatureDispatcher _dispatcher;

    public AdminMatchesController(FeatureDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    ///     Create match for event type (SuperAdmin only)
    /// </summary>
    [HttpPost("event-types/{eventTypeId:guid}/matches")]
    public async Task<IActionResult> CreateMatch(Guid eventTypeId, [FromBody] CreateMatchRequest request)
    {
        var userId = GetUserId();

        var command = new CreateMatchCommand
        {
            EventTypeId = eventTypeId,
            Country1 = request.Country1,
            Country2 = request.Country2,
            MatchDateTime = request.MatchDateTime,
            UserId = userId
        };

        var result = await _dispatcher.HandleAsync<CreateMatchCommand, CreateMatchResult>(command);

        if (result.IsError)
            return MapErrors(result.Errors);

        var match = result.Value;
        return Created($"/api/admin/event-types/{match.EventTypeId}/matches/{match.Id}", match.ToMatchCreatedResponse());
    }

    /// <summary>
    ///     Update match (SuperAdmin only)
    /// </summary>
    [HttpPut("matches/{matchId:guid}")]
    public async Task<IActionResult> UpdateMatch(Guid matchId, [FromBody] UpdateMatchRequest request)
    {
        var userId = GetUserId();

        var command = new UpdateMatchCommand
        {
            MatchId = matchId,
            Country1 = request.Country1,
            Country2 = request.Country2,
            MatchDateTime = request.MatchDateTime,
            UserId = userId
        };

        var result = await _dispatcher.HandleAsync<UpdateMatchCommand, Success>(command);

        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(new SuccessResponse("Mecz został zaktualizowany"));
    }

    /// <summary>
    ///     Delete match (SuperAdmin only)
    /// </summary>
    [HttpDelete("matches/{matchId:guid}")]
    public async Task<IActionResult> DeleteMatch(Guid matchId)
    {
        var userId = GetUserId();

        var command = new DeleteMatchCommand
        {
            MatchId = matchId,
            UserId = userId
        };

        var result = await _dispatcher.HandleAsync<DeleteMatchCommand, Success>(command);

        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(new SuccessResponse("Mecz został usunięty"));
    }

    /// <summary>
    ///     Set match result (SuperAdmin only)
    /// </summary>
    [HttpPost("matches/{matchId:guid}/result")]
    public async Task<IActionResult> SetMatchResult(Guid matchId, [FromBody] SetMatchResultRequest request)
    {
        var userId = GetUserId();

        var command = new SetMatchResultCommand
        {
            MatchId = matchId,
            Result = request.Result,
            UserId = userId
        };

        var result = await _dispatcher.HandleAsync<SetMatchResultCommand, Success>(command);

        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(new SuccessResponse("Wynik meczu został ustawiony"));
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("User ID not found in claims");

        return userId;
    }
}
