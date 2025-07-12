using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TypowanieMeczy.Api.Models;
using TypowanieMeczy.Domain.Interfaces;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Domain.Services;
using TypowanieMeczy.Domain.Entities;

namespace TypowanieMeczy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _matchService;
    private readonly IMatchRepository _matchRepository;
    private readonly IBetService _betService;
    private readonly ILogger<MatchesController> _logger;

    public MatchesController(
        IMatchService matchService,
        IMatchRepository matchRepository,
        IBetService betService,
        ILogger<MatchesController> logger)
    {
        _matchService = matchService;
        _matchRepository = matchRepository;
        _betService = betService;
        _logger = logger;
    }

    [HttpGet("table/{tableId}")]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetTableMatches(string tableId)
    {
        try
        {
            var tableIdValue = TableId.FromString(tableId);
            var userId = GetCurrentUserId();

            var matches = await _matchRepository.GetByTableIdAsync(tableIdValue);
            var matchDtos = matches.Select(MatchDto.FromEntity);
            return Ok(matchDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting matches for table {TableId}", tableId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("table/{tableId}/upcoming")]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetUpcomingMatches(string tableId)
    {
        try
        {
            var tableIdValue = TableId.FromString(tableId);
            var matches = await _matchRepository.GetUpcomingByTableIdAsync(tableIdValue);
            var matchDtos = matches.Select(MatchDto.FromEntity);
            return Ok(matchDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upcoming matches for table {TableId}", tableId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("table/{tableId}/finished")]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetFinishedMatches(string tableId)
    {
        try
        {
            var tableIdValue = TableId.FromString(tableId);
            var matches = await _matchRepository.GetFinishedByTableIdAsync(tableIdValue);
            var matchDtos = matches.Select(MatchDto.FromEntity);
            return Ok(matchDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting finished matches for table {TableId}", tableId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MatchDto>> GetMatch(string id)
    {
        try
        {
            var matchId = MatchId.FromString(id);
            var match = await _matchRepository.GetByIdAsync(matchId);
            
            if (match == null)
                return NotFound();

            return Ok(MatchDto.FromEntity(match));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting match {MatchId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<MatchDto>> CreateMatch(CreateMatchRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var tableId = TableId.FromString(request.TableId);
            var country1 = new Country(request.Country1);
            var country2 = new Country(request.Country2);
            var matchDateTime = new MatchDateTime(request.MatchDateTime);

            var match = new Match(tableId, country1, country2, matchDateTime, userId);
            await _matchRepository.AddAsync(match);

            _logger.LogInformation("Match {Country1} vs {Country2} created by user {UserId}", 
                request.Country1, request.Country2, userId);

            return CreatedAtAction(nameof(GetMatch), new { id = match.Id }, MatchDto.FromEntity(match));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating match");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/start")]
    [Authorize(Roles = "Admin")] // Only admins can start matches
    public async Task<IActionResult> StartMatch(string id)
    {
        try
        {
            var matchId = MatchId.FromString(id);
            var canStart = await _matchService.CanStartMatchAsync(matchId);
            
            if (!canStart)
                return BadRequest("Cannot start match");

            await _matchService.StartMatchAsync(matchId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting match {MatchId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/finish")]
    [Authorize(Roles = "Admin")] // Only admins can finish matches
    public async Task<IActionResult> FinishMatch(string id, FinishMatchRequest request)
    {
        try
        {
            var matchId = MatchId.FromString(id);
            var result = new MatchResult(request.Result);
            var canFinish = await _matchService.CanFinishMatchAsync(matchId);
            
            if (!canFinish)
                return BadRequest("Cannot finish match");

            await _matchService.FinishMatchAsync(matchId, result);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finishing match {MatchId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // Only admins can delete matches
    public async Task<IActionResult> DeleteMatch(string id)
    {
        try
        {
            var matchId = MatchId.FromString(id);
            await _matchRepository.DeleteAsync(matchId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting match {MatchId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/bet")]
    public async Task<IActionResult> PlaceBet(string id, [FromBody] PlaceBetRequest request)
    {
        try
        {
            var matchId = MatchId.FromString(id);
            var userId = GetCurrentUserId();

            var canBet = await _betService.CanUserBetAsync(userId, matchId);
            if (!canBet)
                return BadRequest("Cannot place bet");

            var isBettingClosed = await _betService.IsBettingClosedAsync(matchId);
            if (isBettingClosed)
                return BadRequest("Betting is closed for this match");

            var prediction = new MatchPrediction(request.Prediction);
            var bet = new Bet(userId, matchId, prediction);
            await _betService.PlaceBetAsync(bet);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error placing bet for match {MatchId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}/bets")]
    public async Task<ActionResult<IEnumerable<BetDto>>> GetMatchBets(string id)
    {
        try
        {
            var matchId = MatchId.FromString(id);
            var bets = await _betService.GetMatchBetsAsync(matchId);
            var betDtos = bets.Select(BetDto.FromEntity);
            return Ok(betDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bets for match {MatchId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    private UserId GetCurrentUserId()
    {
        var userId = HttpContext.Items["UserId"] as UserId;
        if (userId == null)
            throw new InvalidOperationException("User ID not found in context");

        return userId;
    }
} 