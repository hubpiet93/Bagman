using TypowanieMeczy.Domain.Interfaces;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Infrastructure.Services;
using TypowanieMeczy.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Postgrest.Constants;

namespace TypowanieMeczy.Infrastructure.Repositories;

public class SupabaseStatisticsRepository : IStatisticsRepository
{
    private readonly ISupabaseClient _supabaseClient;
    private readonly ILogger<SupabaseStatisticsRepository> _logger;

    public SupabaseStatisticsRepository(ISupabaseClient supabaseClient, ILogger<SupabaseStatisticsRepository> logger)
    {
        _supabaseClient = supabaseClient;
        _logger = logger;
    }

    public async Task<decimal> GetUserTotalWinningsAsync(UserId userId, TableId tableId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseUserStats>()
                .Select("total_won")
                .Filter("user_id", Postgrest.Constants.Operator.Equals, userId.ToString())
                .Filter("table_id", Postgrest.Constants.Operator.Equals, tableId.ToString())
                .GetAsync();

            var stats = response.FirstOrDefault();
            return stats?.TotalWon ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total winnings for user {UserId} in table {TableId}", userId, tableId);
            return 0;
        }
    }

    public async Task<int> GetUserMatchesPlayedAsync(UserId userId, TableId tableId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseUserStats>()
                .Select("matches_played")
                .Filter("user_id", Postgrest.Constants.Operator.Equals, userId.ToString())
                .Filter("table_id", Postgrest.Constants.Operator.Equals, tableId.ToString())
                .GetAsync();

            var stats = response.FirstOrDefault();
            return stats?.MatchesPlayed ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting matches played for user {UserId} in table {TableId}", userId, tableId);
            return 0;
        }
    }

    public async Task<int> GetUserBetsPlacedAsync(UserId userId, TableId tableId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseUserStats>()
                .Select("bets_placed")
                .Filter("user_id", Postgrest.Constants.Operator.Equals, userId.ToString())
                .Filter("table_id", Postgrest.Constants.Operator.Equals, tableId.ToString())
                .GetAsync();

            var stats = response.FirstOrDefault();
            return stats?.BetsPlaced ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bets placed for user {UserId} in table {TableId}", userId, tableId);
            return 0;
        }
    }

    public async Task<int> GetUserPoolsWonAsync(UserId userId, TableId tableId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseUserStats>()
                .Select("pools_won")
                .Filter("user_id", Postgrest.Constants.Operator.Equals, userId.ToString())
                .Filter("table_id", Postgrest.Constants.Operator.Equals, tableId.ToString())
                .GetAsync();

            var stats = response.FirstOrDefault();
            return stats?.PoolsWon ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pools won for user {UserId} in table {TableId}", userId, tableId);
            return 0;
        }
    }
} 