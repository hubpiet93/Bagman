using System.Security.Claims;
using Bagman.Application.Common;
using Bagman.Application.Features.Matches.CreateMatch;
using Bagman.Application.Features.Matches.DeleteMatch;
using Bagman.Application.Features.Matches.GetMatchDetails;
using Bagman.Application.Features.Matches.SetMatchResult;
using Bagman.Application.Features.Matches.UpdateMatch;
using Bagman.Contracts.Models.Tables;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bagman.Api.Controllers;

[ApiController]
[Route("api/tables/{tableId}/matches")]
[Authorize]
public class MatchesController : AppControllerBase
{
    private readonly FeatureDispatcher _dispatcher;

    public MatchesController(FeatureDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    ///     Dodanie nowego meczu (tylko admin)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateMatch(Guid tableId, [FromBody] CreateMatchRequest request)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var result = await _dispatcher.HandleAsync<CreateMatchCommand, CreateMatchResult>(
            new CreateMatchCommand
            {
                TableId = tableId,
                Country1 = request.Country1,
                Country2 = request.Country2,
                MatchDateTime = request.MatchDateTime,
                UserId = userId.Value
            });
        
        if (result.IsError)
            return MapErrors(result.Errors);

        var response = new MatchResponse
        {
            Id = result.Value.Id,
            TableId = result.Value.TableId,
            Country1 = result.Value.Country1,
            Country2 = result.Value.Country2,
            MatchDateTime = result.Value.MatchDateTime,
            Result = null,
            Status = result.Value.Status,
            Started = DateTime.UtcNow >= result.Value.MatchDateTime,
            CreatedAt = result.Value.CreatedAt
        };

        return CreatedAtAction(nameof(GetMatchDetails), new
        {
            tableId,
            matchId = result.Value.Id
        }, response);
    }

    /// <summary>
    ///     Pobranie szczegółów meczu
    /// </summary>
    [HttpGet("{matchId}")]
    public async Task<IActionResult> GetMatchDetails(Guid tableId, Guid matchId)
    {
        var result = await _dispatcher.HandleAsync<GetMatchDetailsQuery, MatchDetailsResult>(
            new GetMatchDetailsQuery { MatchId = matchId });
        
        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(new MatchResponse
        {
            Id = result.Value.Id,
            TableId = result.Value.TableId,
            Country1 = result.Value.Country1,
            Country2 = result.Value.Country2,
            MatchDateTime = result.Value.MatchDateTime,
            Result = result.Value.Result,
            Status = result.Value.Status,
            Started = DateTime.UtcNow >= result.Value.MatchDateTime,
            CreatedAt = result.Value.CreatedAt
        });
    }

    /// <summary>
    ///     Aktualizacja meczu (tylko admin, przed rozpoczęciem)
    /// </summary>
    [HttpPut("{matchId}")]
    public async Task<IActionResult> UpdateMatch(Guid tableId, Guid matchId, [FromBody] UpdateMatchRequest request)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var result = await _dispatcher.HandleAsync<UpdateMatchCommand, Success>(
            new UpdateMatchCommand
            {
                MatchId = matchId,
                Country1 = request.Country1,
                Country2 = request.Country2,
                MatchDateTime = request.MatchDateTime,
                UserId = userId.Value
            });
        
        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(new {message = "Match updated successfully"});
    }

    /// <summary>
    ///     Usunięcie meczu (tylko admin, przed rozpoczęciem)
    /// </summary>
    [HttpDelete("{matchId}")]
    public async Task<IActionResult> DeleteMatch(Guid tableId, Guid matchId)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var result = await _dispatcher.HandleAsync<DeleteMatchCommand, Success>(
            new DeleteMatchCommand
            {
                MatchId = matchId,
                UserId = userId.Value
            });
        
        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(new {message = "Match deleted successfully"});
    }

    /// <summary>
    ///     Wprowadzenie wyniku meczu (tylko admin)
    /// </summary>
    [HttpPut("{matchId}/result")]
    public async Task<IActionResult> SetMatchResult(Guid tableId, Guid matchId, [FromBody] SetMatchResultRequest request)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var result = $"{request.Score1}:{request.Score2}";
        var setResultResponse = await _dispatcher.HandleAsync<SetMatchResultCommand, Success>(
            new SetMatchResultCommand
            {
                MatchId = matchId,
                Result = result,
                UserId = userId.Value
            });

        if (setResultResponse.IsError)
            return MapErrors(setResultResponse.Errors);

        return Ok(new {message = "Match result set successfully"});
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;

        return null;
    }
}
