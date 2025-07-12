using TypowanieMeczy.Domain.Services;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Domain.Interfaces;

namespace TypowanieMeczy.Infrastructure.Services;

public class PoolService : IPoolService
{
    private readonly IPoolRepository _poolRepository;
    private readonly IMatchRepository _matchRepository;

    public PoolService(IPoolRepository poolRepository, IMatchRepository matchRepository)
    {
        _poolRepository = poolRepository;
        _matchRepository = matchRepository;
    }

    public async Task<decimal> CalculatePoolAmountAsync(MatchId matchId)
    {
        var pool = await _poolRepository.GetByMatchIdAsync(matchId);
        return pool?.Amount ?? 0;
    }

    public async Task DistributePoolAsync(MatchId matchId, IEnumerable<UserId> winners)
    {
        var pool = await _poolRepository.GetByMatchIdAsync(matchId);
        if (pool == null) return;
        // Implement distribution logic
    }

    public async Task RolloverPoolAsync(MatchId matchId)
    {
        var pool = await _poolRepository.GetByMatchIdAsync(matchId);
        if (pool == null) return;
        // Implement rollover logic
    }

    public async Task<decimal> GetUserWinningsAsync(MatchId matchId, UserId userId)
    {
        var pool = await _poolRepository.GetByMatchIdAsync(matchId);
        if (pool == null) return 0;
        // Implement logic to get user winnings
        return 0;
    }
} 