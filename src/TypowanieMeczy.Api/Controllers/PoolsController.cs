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
public class PoolsController : ControllerBase
{
    private readonly IPoolRepository _poolRepository;
    private readonly IPoolService _poolService;
    private readonly IAuthService _authService;
    private readonly ILogger<PoolsController> _logger;

    public PoolsController(
        IPoolRepository poolRepository,
        IPoolService poolService,
        IAuthService authService,
        ILogger<PoolsController> logger)
    {
        _poolRepository = poolRepository;
        _poolService = poolService;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PoolDto>> GetPool(string id)
    {
        try
        {
            var poolId = PoolId.FromString(id);
            var pool = await _poolRepository.GetByIdAsync(poolId);
            if (pool == null)
                return NotFound();

            var poolDto = new PoolDto
            {
                Id = pool.Id.ToString(),
                MatchId = pool.MatchId.ToString(),
                Amount = pool.Amount,
                Status = pool.Status.ToString(),
                CreatedAt = pool.CreatedAt,
                Winners = pool.Winners.Select(w => w.ToString()).ToList()
            };

            return Ok(poolDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pool");
            return BadRequest("Invalid pool ID");
        }
    }

    [HttpGet("match/{matchId}")]
    public async Task<ActionResult<PoolDto>> GetPoolByMatch(string matchId)
    {
        try
        {
            var matchIdValue = MatchId.FromString(matchId);
            var pool = await _poolRepository.GetByMatchIdAsync(matchIdValue);
            if (pool == null)
                return NotFound();

            var poolDto = new PoolDto
            {
                Id = pool.Id.ToString(),
                MatchId = pool.MatchId.ToString(),
                Amount = pool.Amount,
                Status = pool.Status.ToString(),
                CreatedAt = pool.CreatedAt,
                Winners = pool.Winners.Select(w => w.ToString()).ToList()
            };

            return Ok(poolDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pool by match");
            return BadRequest("Invalid match ID");
        }
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<PoolDto>>> GetActivePools()
    {
        try
        {
            var pools = await _poolRepository.GetActivePoolsAsync();

            var poolDtos = pools.Select(pool => new PoolDto
            {
                Id = pool.Id.ToString(),
                MatchId = pool.MatchId.ToString(),
                Amount = pool.Amount,
                Status = pool.Status.ToString(),
                CreatedAt = pool.CreatedAt,
                Winners = pool.Winners.Select(w => w.ToString()).ToList()
            });

            return Ok(poolDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active pools");
            return BadRequest("Error retrieving active pools");
        }
    }

    [HttpGet("expired")]
    public async Task<ActionResult<IEnumerable<PoolDto>>> GetExpiredPools()
    {
        try
        {
            var pools = await _poolRepository.GetExpiredPoolsAsync();

            var poolDtos = pools.Select(pool => new PoolDto
            {
                Id = pool.Id.ToString(),
                MatchId = pool.MatchId.ToString(),
                Amount = pool.Amount,
                Status = pool.Status.ToString(),
                CreatedAt = pool.CreatedAt,
                Winners = pool.Winners.Select(w => w.ToString()).ToList()
            });

            return Ok(poolDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expired pools");
            return BadRequest("Error retrieving expired pools");
        }
    }

    [HttpPost("match/{matchId}/calculate")]
    public async Task<ActionResult<decimal>> CalculatePoolAmount(string matchId)
    {
        try
        {
            var matchIdValue = MatchId.FromString(matchId);
            var amount = await _poolService.CalculatePoolAmountAsync(matchIdValue);
            return Ok(amount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating pool amount");
            return BadRequest("Error calculating pool amount");
        }
    }

    [HttpPost("match/{matchId}/distribute")]
    public async Task<ActionResult> DistributePool(string matchId, [FromBody] List<string> winnerIds)
    {
        try
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var matchIdValue = MatchId.FromString(matchId);
            var winners = winnerIds.Select(id => UserId.FromString(id)).ToList();

            await _poolService.DistributePoolAsync(matchIdValue, winners);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error distributing pool");
            return BadRequest("Error distributing pool");
        }
    }

    [HttpPost("match/{matchId}/rollover")]
    public async Task<ActionResult> RolloverPool(string matchId)
    {
        try
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var matchIdValue = MatchId.FromString(matchId);
            await _poolService.RolloverPoolAsync(matchIdValue);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling over pool");
            return BadRequest("Error rolling over pool");
        }
    }

    [HttpGet("match/{matchId}/winnings/{userId}")]
    public async Task<ActionResult<decimal>> GetUserWinnings(string matchId, string userId)
    {
        try
        {
            var matchIdValue = MatchId.FromString(matchId);
            var userIdValue = UserId.FromString(userId);
            var winnings = await _poolService.GetUserWinningsAsync(matchIdValue, userIdValue);
            return Ok(winnings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user winnings");
            return BadRequest("Error getting user winnings");
        }
    }
} 