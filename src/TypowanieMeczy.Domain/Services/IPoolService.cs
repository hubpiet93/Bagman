using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Services;

public interface IPoolService
{
    Task<decimal> CalculatePoolAmountAsync(MatchId matchId);
    Task DistributePoolAsync(MatchId matchId, IEnumerable<UserId> winners);
    Task RolloverPoolAsync(MatchId matchId);
    Task<decimal> GetUserWinningsAsync(MatchId matchId, UserId userId);
} 