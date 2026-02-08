using System.Security.Claims;
using Bagman.Api.Controllers.Mappers;
using Bagman.Application.Common;
using Bagman.Application.Features.Bets.DeleteBet;
using Bagman.Application.Features.Bets.GetUserBet;
using Bagman.Application.Features.Bets.PlaceBet;
using Bagman.Contracts.Models;
using Bagman.Contracts.Models.Tables;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bagman.Api.Controllers;

[ApiController]
[Route("api/tables/{tableId}/matches/{matchId}/bets")]
[Authorize]
public class BetsController : AppControllerBase
{
    private readonly FeatureDispatcher _dispatcher;

    public BetsController(FeatureDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    ///     Dodanie/aktualizacja typu na mecz
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PlaceBet(Guid tableId, Guid matchId, [FromBody] PlaceBetRequest request)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var result = await _dispatcher.HandleAsync<PlaceBetCommand, PlaceBetResult>(
            new PlaceBetCommand
            {
                TableId = tableId,
                MatchId = matchId,
                UserId = userId.Value,
                Prediction = request.Prediction
            });

        if (result.IsError)
            return MapErrors(result.Errors);

        var response = result.Value.ToBetResponse();

        return CreatedAtAction(nameof(GetUserBet), new
        {
            tableId,
            matchId
        }, response);
    }

    /// <summary>
    ///     Pobranie typu użytkownika na mecz
    /// </summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetUserBet(Guid tableId, Guid matchId)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var result = await _dispatcher.HandleAsync<GetUserBetQuery, UserBetResult>(
            new GetUserBetQuery
            {
                MatchId = matchId,
                UserId = userId.Value
            });

        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(result.Value.ToBetResponse());
    }

    /// <summary>
    ///     Usunięcie typu (przed rozpoczęciem meczu)
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> DeleteBet(Guid tableId, Guid matchId)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var result = await _dispatcher.HandleAsync<DeleteBetCommand, Success>(
            new DeleteBetCommand
            {
                MatchId = matchId,
                UserId = userId.Value
            });

        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(new SuccessResponse("Bet deleted successfully"));
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;

        return null;
    }
}
