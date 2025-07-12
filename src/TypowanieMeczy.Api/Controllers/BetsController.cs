using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TypowanieMeczy.Api.Models;
using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.Interfaces;
using TypowanieMeczy.Domain.Services;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Infrastructure.Services;

namespace TypowanieMeczy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BetsController : ControllerBase
{
    private readonly IBetRepository _betRepository;
    private readonly IBetService _betService;
    private readonly IAuthService _authService;
    private readonly ILogger<BetsController> _logger;

    public BetsController(
        IBetRepository betRepository,
        IBetService betService,
        IAuthService authService,
        ILogger<BetsController> logger)
    {
        _betRepository = betRepository;
        _betService = betService;
        _authService = authService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<BetDto>> PlaceBet([FromBody] CreateBetRequest request)
    {
        try
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var userId = UserId.FromString(currentUser.Id);
            var matchId = MatchId.FromString(request.MatchId);
            var prediction = new MatchPrediction(request.Prediction);

            if (!await _betService.CanUserBetAsync(userId, matchId))
                return BadRequest("Cannot place bet - betting may be closed or user already has a bet");

            var existingBet = await _betRepository.GetByUserAndMatchAsync(userId, matchId);
            if (existingBet != null)
                return BadRequest("User already has a bet for this match");

            var bet = new Bet(userId, matchId, prediction);
            await _betRepository.AddAsync(bet);

            var betDto = new BetDto
            {
                Id = bet.Id.ToString(),
                UserId = bet.UserId.ToString(),
                MatchId = bet.MatchId.ToString(),
                Prediction = bet.Prediction.ToString(),
                EditedAt = bet.EditedAt,
                IsWinner = bet.IsWinner,
                UserName = currentUser.Email ?? "Unknown"
            };

            return CreatedAtAction(nameof(GetBet), new { id = bet.Id }, betDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error placing bet");
            return BadRequest("Invalid bet data");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BetDto>> UpdateBet(string id, [FromBody] UpdateBetRequest request)
    {
        try
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var betId = BetId.FromString(id);
            var bet = await _betRepository.GetByIdAsync(betId);
            if (bet == null)
                return NotFound();

            var userId = UserId.FromString(currentUser.Id);
            if (bet.UserId != userId)
                return Forbid();

            if (await _betService.IsBettingClosedAsync(bet.MatchId))
                return BadRequest("Betting is closed for this match");

            var prediction = new MatchPrediction(request.Prediction);
            bet.UpdatePrediction(prediction);
            await _betRepository.UpdateAsync(bet);

            var betDto = new BetDto
            {
                Id = bet.Id.ToString(),
                UserId = bet.UserId.ToString(),
                MatchId = bet.MatchId.ToString(),
                Prediction = bet.Prediction.ToString(),
                EditedAt = bet.EditedAt,
                IsWinner = bet.IsWinner,
                UserName = currentUser.Email ?? "Unknown"
            };

            return Ok(betDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating bet");
            return BadRequest("Invalid bet data");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BetDto>> GetBet(string id)
    {
        try
        {
            var betId = BetId.FromString(id);
            var bet = await _betRepository.GetByIdAsync(betId);
            if (bet == null)
                return NotFound();

            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var betDto = new BetDto
            {
                Id = bet.Id.ToString(),
                UserId = bet.UserId.ToString(),
                MatchId = bet.MatchId.ToString(),
                Prediction = bet.Prediction.ToString(),
                EditedAt = bet.EditedAt,
                IsWinner = bet.IsWinner,
                UserName = currentUser.Email ?? "Unknown"
            };

            return Ok(betDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bet");
            return BadRequest("Invalid bet ID");
        }
    }

    [HttpGet("match/{matchId}")]
    public async Task<ActionResult<IEnumerable<BetDto>>> GetMatchBets(string matchId)
    {
        try
        {
            var matchIdValue = MatchId.FromString(matchId);
            var bets = await _betRepository.GetByMatchIdAsync(matchIdValue);

            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var betDtos = bets.Select(bet => new BetDto
            {
                Id = bet.Id.ToString(),
                UserId = bet.UserId.ToString(),
                MatchId = bet.MatchId.ToString(),
                Prediction = bet.Prediction.ToString(),
                EditedAt = bet.EditedAt,
                IsWinner = bet.IsWinner,
                UserName = currentUser.Email ?? "Unknown"
            });

            return Ok(betDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting match bets");
            return BadRequest("Invalid match ID");
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<BetDto>>> GetUserBets(string userId)
    {
        try
        {
            var userIdValue = UserId.FromString(userId);
            var bets = await _betRepository.GetByUserIdAsync(userIdValue);

            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var betDtos = bets.Select(bet => new BetDto
            {
                Id = bet.Id.ToString(),
                UserId = bet.UserId.ToString(),
                MatchId = bet.MatchId.ToString(),
                Prediction = bet.Prediction.ToString(),
                EditedAt = bet.EditedAt,
                IsWinner = bet.IsWinner,
                UserName = currentUser.Email ?? "Unknown"
            });

            return Ok(betDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user bets");
            return BadRequest("Invalid user ID");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBet(string id)
    {
        try
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var betId = BetId.FromString(id);
            var bet = await _betRepository.GetByIdAsync(betId);
            if (bet == null)
                return NotFound();

            var userId = UserId.FromString(currentUser.Id);
            if (bet.UserId != userId)
                return Forbid();

            await _betRepository.DeleteAsync(betId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting bet");
            return BadRequest("Invalid bet ID");
        }
    }
} 