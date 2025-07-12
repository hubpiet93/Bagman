using TypowanieMeczy.Domain.Services;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Domain.Interfaces;

namespace TypowanieMeczy.Infrastructure.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IStatisticsRepository _statisticsRepository;

    public StatisticsService(IStatisticsRepository statisticsRepository)
    {
        _statisticsRepository = statisticsRepository;
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
} 