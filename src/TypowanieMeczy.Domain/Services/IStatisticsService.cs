using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Services;

public interface IStatisticsService
{
    Task<decimal> GetUserTotalWinningsAsync(UserId userId, TableId tableId);
    Task<int> GetUserMatchesPlayedAsync(UserId userId, TableId tableId);
    Task<int> GetUserBetsPlacedAsync(UserId userId, TableId tableId);
    Task<int> GetUserPoolsWonAsync(UserId userId, TableId tableId);
} 