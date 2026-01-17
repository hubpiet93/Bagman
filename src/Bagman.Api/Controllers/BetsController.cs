using Bagman.Contracts.Models.Tables;
using Bagman.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bagman.Api.Controllers;

[ApiController]
[Route("api/tables/{tableId}/matches/{matchId}/bets")]
[Authorize]
public class BetsController : AppControllerBase
{
    private readonly IBetService _betService;

    public BetsController(IBetService betService)
    {
        _betService = betService;
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

        var result = await _betService.PlaceBetAsync(matchId, userId.Value, request.Prediction);
        if (result.IsError)
            return MapErrors(result.Errors);

        var response = new BetResponse
        {
            Id = result.Value.Id,
            UserId = result.Value.UserId,
            MatchId = result.Value.MatchId,
            Prediction = result.Value.Prediction,
            EditedAt = result.Value.EditedAt
        };

        return CreatedAtAction(nameof(GetUserBet), new { tableId = tableId, matchId = matchId }, response);
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

        var result = await _betService.GetBetAsync(matchId, userId.Value);
        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(new BetResponse
        {
            Id = result.Value.Id,
            UserId = result.Value.UserId,
            MatchId = result.Value.MatchId,
            Prediction = result.Value.Prediction,
            EditedAt = result.Value.EditedAt
        });
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

        var result = await _betService.DeleteBetAsync(matchId, userId.Value);
        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(new { message = "Bet deleted successfully" });
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;

        return null;
    }
}
