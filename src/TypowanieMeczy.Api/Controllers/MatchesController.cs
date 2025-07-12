using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TypowanieMeczy.Api.Models;
using TypowanieMeczy.Domain.Services;
using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Api.Controllers;

[ApiController]
[Route("api/v1/tables/{tableId}/[controller]")]
[Authorize]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _matchService;
    private readonly ILogger<MatchesController> _logger;

    public MatchesController(IMatchService matchService, ILogger<MatchesController> logger)
    {
        _matchService = matchService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<MatchDto>> CreateMatch(string tableId, [FromBody] CreateMatchRequest request)
    {
        try
        {
            var tableIdValue = TableId.FromString(tableId);
            var userId = GetCurrentUserId();
            var country1 = new Country(request.Country1);
            var country2 = new Country(request.Country2);
            var matchDateTime = new MatchDateTime(request.MatchDateTime);

            var match = await _matchService.CreateMatchAsync(tableIdValue, country1, country2, matchDateTime, userId);
            
            return CreatedAtAction(nameof(GetMatch), new { tableId, id = match.Id }, MatchDto.FromEntity(match));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating match for table {TableId}", tableId);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetTableMatches(string tableId)
    {
        try
        {
            var tableIdValue = TableId.FromString(tableId);
            var userId = GetCurrentUserId();

            var matches = await _matchService.GetTableMatchesAsync(tableIdValue, userId);
            
            return Ok(matches.Select(MatchDto.FromEntity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting matches for table {TableId}", tableId);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetUpcomingMatches(string tableId)
    {
        try
        {
            var tableIdValue = TableId.FromString(tableId);
            var userId = GetCurrentUserId();

            var matches = await _matchService.GetUpcomingMatchesAsync(tableIdValue, userId);
            
            return Ok(matches.Select(MatchDto.FromEntity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upcoming matches for table {TableId}", tableId);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("finished")]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetFinishedMatches(string tableId)
    {
        try
        {
            var tableIdValue = TableId.FromString(tableId);
            var userId = GetCurrentUserId();

            var matches = await _matchService.GetFinishedMatchesAsync(tableIdValue, userId);
            
            return Ok(matches.Select(MatchDto.FromEntity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting finished matches for table {TableId}", tableId);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MatchDto>> GetMatch(string tableId, string id)
    {
        try
        {
            var matchId = MatchId.FromString(id);
            var userId = GetCurrentUserId();

            var match = await _matchService.GetMatchDetailsAsync(matchId, userId);
            if (match == null)
                return NotFound();

            return Ok(MatchDto.FromEntity(match));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting match {MatchId} for table {TableId}", id, tableId);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}/result")]
    public async Task<ActionResult<MatchDto>> UpdateMatchResult(string tableId, string id, [FromBody] UpdateMatchResultRequest request)
    {
        try
        {
            var matchId = MatchId.FromString(id);
            var userId = GetCurrentUserId();
            var result = new MatchResult(request.Result);

            var match = await _matchService.UpdateMatchResultAsync(matchId, result, userId);
            
            return Ok(MatchDto.FromEntity(match));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating match result {MatchId} for table {TableId}", id, tableId);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMatch(string tableId, string id)
    {
        try
        {
            var matchId = MatchId.FromString(id);
            var userId = GetCurrentUserId();

            await _matchService.DeleteMatchAsync(matchId, userId);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting match {MatchId} for table {TableId}", id, tableId);
            return BadRequest(new { error = ex.Message });
        }
    }

    private UserId GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User ID not found in token");

        return UserId.FromString(userIdClaim);
    }
} 