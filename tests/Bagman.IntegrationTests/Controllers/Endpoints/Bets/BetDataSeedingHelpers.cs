using Bagman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Bets;

/// <summary>
///     Helper methods for seeding Bet data directly into the database.
///     Used for testing scenarios that require bets on already-started or finished matches.
/// </summary>
public static class BetDataSeedingHelpers
{
    /// <summary>
    ///     Seeds a Bet record directly into the database using raw SQL.
    ///     This bypasses domain validation (e.g., match must not be started).
    /// </summary>
    public static async Task SeedBetAsync(
        IServiceProvider services,
        Guid userId,
        Guid matchId,
        string prediction)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.ExecuteSqlRawAsync(@"
            INSERT INTO bets (id, user_id, match_id, prediction, edited_at)
            VALUES ({0}, {1}, {2}, {3}, {4})",
            Guid.NewGuid(), userId, matchId, prediction, DateTime.UtcNow);
    }
}
