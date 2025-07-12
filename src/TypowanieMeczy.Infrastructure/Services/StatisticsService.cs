using TypowanieMeczy.Domain.Services;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Domain.Interfaces;

namespace TypowanieMeczy.Infrastructure.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly IBetRepository _betRepository;
    private readonly IPoolRepository _poolRepository;
    private readonly IMatchRepository _matchRepository;

    public StatisticsService(
        IStatisticsRepository statisticsRepository,
        IBetRepository betRepository,
        IPoolRepository poolRepository,
        IMatchRepository matchRepository)
    {
        _statisticsRepository = statisticsRepository;
        _betRepository = betRepository;
        _poolRepository = poolRepository;
        _matchRepository = matchRepository;
    }

    public async Task<decimal> GetUserTotalWinningsAsync(UserId userId, TableId tableId)
    {
        return await _statisticsRepository.GetUserTotalWinningsAsync(userId, tableId);
    }

    public async Task<int> GetUserMatchesPlayedAsync(UserId userId, TableId tableId)
    {
        return await _statisticsRepository.GetUserMatchesPlayedAsync(userId, tableId);
    }

    public async Task<int> GetUserBetsPlacedAsync(UserId userId, TableId tableId)
    {
        return await _statisticsRepository.GetUserBetsPlacedAsync(userId, tableId);
    }

    public async Task<int> GetUserPoolsWonAsync(UserId userId, TableId tableId)
    {
        return await _statisticsRepository.GetUserPoolsWonAsync(userId, tableId);
    }

    public async Task<double> GetUserWinRateAsync(UserId userId, TableId tableId)
    {
        var matchesPlayed = await GetUserMatchesPlayedAsync(userId, tableId);
        var poolsWon = await GetUserPoolsWonAsync(userId, tableId);

        if (matchesPlayed == 0) return 0.0;
        return (double)poolsWon / matchesPlayed * 100;
    }

    public async Task<decimal> GetUserAverageWinningsAsync(UserId userId, TableId tableId)
    {
        var totalWinnings = await GetUserTotalWinningsAsync(userId, tableId);
        var poolsWon = await GetUserPoolsWonAsync(userId, tableId);

        if (poolsWon == 0) return 0;
        return totalWinnings / poolsWon;
    }

    public async Task<IEnumerable<UserStatistics>> GetTableLeaderboardAsync(TableId tableId)
    {
        // This would require a more complex query to get all users in a table
        // For now, we'll return an empty list
        // In a real implementation, you would:
        // 1. Get all users in the table
        // 2. Calculate statistics for each user
        // 3. Order by total winnings
        return Enumerable.Empty<UserStatistics>();
    }

    public async Task<UserStatistics> GetUserStatisticsAsync(UserId userId, TableId tableId)
    {
        var totalWinnings = await GetUserTotalWinningsAsync(userId, tableId);
        var matchesPlayed = await GetUserMatchesPlayedAsync(userId, tableId);
        var betsPlaced = await GetUserBetsPlacedAsync(userId, tableId);
        var poolsWon = await GetUserPoolsWonAsync(userId, tableId);
        var winRate = await GetUserWinRateAsync(userId, tableId);
        var averageWinnings = await GetUserAverageWinningsAsync(userId, tableId);

        return new UserStatistics
        {
            UserId = userId,
            TableId = tableId,
            TotalWinnings = totalWinnings,
            MatchesPlayed = matchesPlayed,
            BetsPlaced = betsPlaced,
            PoolsWon = poolsWon,
            WinRate = winRate,
            AverageWinnings = averageWinnings
        };
    }
}

public class UserStatistics
{
    public UserId UserId { get; set; }
    public TableId TableId { get; set; }
    public decimal TotalWinnings { get; set; }
    public int MatchesPlayed { get; set; }
    public int BetsPlaced { get; set; }
    public int PoolsWon { get; set; }
    public double WinRate { get; set; }
    public decimal AverageWinnings { get; set; }
} 