using TypowanieMeczy.Domain.Services;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Domain.Interfaces;

namespace TypowanieMeczy.Infrastructure.Services;

public class PoolService : IPoolService
{
    private readonly IPoolRepository _poolRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IBetRepository _betRepository;

    public PoolService(IPoolRepository poolRepository, IMatchRepository matchRepository, IBetRepository betRepository)
    {
        _poolRepository = poolRepository;
        _matchRepository = matchRepository;
        _betRepository = betRepository;
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

        var winnersList = winners.ToList();
        if (!winnersList.Any())
        {
            // No winners - rollover pool
            await RolloverPoolAsync(matchId);
            return;
        }

        var amountPerWinner = pool.Amount / winnersList.Count;
        pool.DistributeToWinners(winnersList, amountPerWinner);
        
        await _poolRepository.UpdateAsync(pool);

        // Update user statistics
        foreach (var winner in winnersList)
        {
            // Update user stats - this would be implemented in a real scenario
            // await _statisticsRepository.UpdateUserWinningsAsync(winner, matchId, amountPerWinner);
        }
    }

    public async Task RolloverPoolAsync(MatchId matchId)
    {
        var pool = await _poolRepository.GetByMatchIdAsync(matchId);
        if (pool == null) return;

        pool.MarkForRollover();
        await _poolRepository.UpdateAsync(pool);
    }

    public async Task<decimal> GetUserWinningsAsync(MatchId matchId, UserId userId)
    {
        var pool = await _poolRepository.GetByMatchIdAsync(matchId);
        if (pool == null) return 0;

        if (pool.Winners.Contains(userId))
        {
            var winnerCount = pool.Winners.Count;
            return winnerCount > 0 ? pool.Amount / winnerCount : 0;
        }

        return 0;
    }
} 