using System.Security.Claims;
using Bagman.Api.Attributes;
using Bagman.Api.Controllers.Mappers;
using Bagman.Application.Common;
using Bagman.Application.Features.Matches.CreateMatch;
using Bagman.Application.Features.Matches.DeleteMatch;
using Bagman.Application.Features.Matches.GetMatchesByEventType;
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
    ///     Get matches for event type (SuperAdmin only)
    /// </summary>
    [HttpGet("event-types/{eventTypeId:guid}/matches")]
    public async Task<IActionResult> GetMatchesByEventType(Guid eventTypeId)
    {
        var userId = GetUserId();

        var query = new GetMatchesByEventTypeQuery
        {
            EventTypeId = eventTypeId,
            UserId = userId
        };

        var result = await _dispatcher.HandleAsync<GetMatchesByEventTypeQuery, List<MatchListItemResult>>(query);

        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(result.Value.Select(m => m.ToMatchResponse()));
    }

    /// <summary>
    ///     Update match (SuperAdmin only)
    /// </summary>
    [HttpPut("event-types/{eventTypeId:guid}/matches/{matchId:guid}")]
    public async Task<IActionResult> UpdateMatch(Guid eventTypeId, Guid matchId, [FromBody] UpdateMatchRequest request)
    {
        var userId = GetUserId();

        var command = new UpdateMatchCommand
        {
            MatchId = matchId,
            EventTypeId = eventTypeId,
            Country1 = request.Country1,
            Country2 = request.Country2,
            MatchDateTime = request.MatchDateTime,
            UserId = userId
        };

        var result = await _dispatcher.HandleAsync<UpdateMatchCommand, UpdateMatchResult>(command);

        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(result.Value.ToMatchResponse());
    }

    /// <summary>
    ///     Delete match (SuperAdmin only)
    /// </summary>
    [HttpDelete("event-types/{eventTypeId:guid}/matches/{matchId:guid}")]
    public async Task<IActionResult> DeleteMatch(Guid eventTypeId, Guid matchId)
    {
        var userId = GetUserId();

        var command = new DeleteMatchCommand
        {
            MatchId = matchId,
            EventTypeId = eventTypeId,
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
    [HttpPut("matches/{matchId:guid}/result")]
    public async Task<IActionResult> SetMatchResult(Guid matchId, [FromBody] SetMatchResultRequest request)
    {
        var userId = GetUserId();

        var command = new SetMatchResultCommand
        {
            MatchId = matchId,
            Result = request.Result,
            UserId = userId
        };

        var result = await _dispatcher.HandleAsync<SetMatchResultCommand, SetMatchResultResult>(command);

        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(result.Value);
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
