using Bagman.Domain.Models;
using Bagman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Pools;

/// <summary>
///     Helper methods for seeding Pool and UserStats data directly into the database.
///     Used for testing dashboard display of pools and stats.
/// </summary>
public static class PoolStatsDataSeedingHelpers
{
    /// <summary>
    ///     Seeds a Pool record into the database using Entity Framework.
    /// </summary>
    /// <param name="services">The service provider for database access.</param>
    /// <param name="matchId">The match ID this pool is for.</param>
    /// <param name="amount">The pool amount.</param>
    /// <param name="status">Pool status: "active", "won", "rollover", or "expired".</param>
    /// <param name="winners">Optional list of winners (userId, amountWon).</param>
    /// <returns>The created pool's ID.</returns>
    public static async Task<Guid> SeedPoolAsync(
        IServiceProvider services,
        Guid matchId,
        decimal amount,
        string status = "active",
        List<(Guid UserId, decimal AmountWon)>? winners = null)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var pool = new Pool
        {
            Id = Guid.NewGuid(),
            MatchId = matchId,
            Amount = amount,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };

        if (winners != null)
            foreach (var (userId, amountWon) in winners)
                pool.Winners.Add(new PoolWinner
                {
                    PoolId = pool.Id,
                    UserId = userId,
                    AmountWon = amountWon
                });

        dbContext.Pools.Add(pool);
        await dbContext.SaveChangesAsync();

        return pool.Id;
    }

    /// <summary>
    ///     Seeds a UserStats record into the database using Entity Framework.
    ///     Uses UPSERT semantics - creates new or updates existing record.
    /// </summary>
    /// <param name="services">The service provider for database access.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="tableId">The table ID.</param>
    /// <param name="matchesPlayed">Number of matches played.</param>
    /// <param name="betsPlaced">Number of bets placed.</param>
    /// <param name="poolsWon">Number of pools won.</param>
    /// <param name="totalWon">Total amount won.</param>
    public static async Task SeedUserStatsAsync(
        IServiceProvider services,
        Guid userId,
        Guid tableId,
        int matchesPlayed = 0,
        int betsPlaced = 0,
        int poolsWon = 0,
        decimal totalWon = 0m)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var existingStats = await dbContext.UserStats
            .FirstOrDefaultAsync(s => s.UserId == userId && s.TableId == tableId);

        if (existingStats != null)
        {
            existingStats.MatchesPlayed = matchesPlayed;
            existingStats.BetsPlaced = betsPlaced;
            existingStats.PoolsWon = poolsWon;
            existingStats.TotalWon = totalWon;
            existingStats.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var stats = new UserStats
            {
                UserId = userId,
                TableId = tableId,
                MatchesPlayed = matchesPlayed,
                BetsPlaced = betsPlaced,
                PoolsWon = poolsWon,
                TotalWon = totalWon,
                UpdatedAt = DateTime.UtcNow
            };
            dbContext.UserStats.Add(stats);
        }

        await dbContext.SaveChangesAsync();
    }
}
