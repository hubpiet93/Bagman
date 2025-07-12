using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Interfaces;

public interface IStatisticsRepository
{
    Task<decimal> GetUserTotalWinningsAsync(UserId userId, TableId tableId);
    Task<int> GetUserMatchesPlayedAsync(UserId userId, TableId tableId);
    Task<int> GetUserBetsPlacedAsync(UserId userId, TableId tableId);
    Task<int> GetUserPoolsWonAsync(UserId userId, TableId tableId);
} 