using Bagman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Bets;

/// <summary>
///     Helper methods for seeding Bet data directly into the database.
///     Used for testing scenarios where matches are already started (past dates)
///     and bets cannot be placed via the API.
/// </summary>
public static class BetSeedingHelpers
{
    /// <summary>
    ///     Seeds a Bet record directly into the database using raw SQL.
    ///     This bypasses domain validation (e.g., match must not have started).
    /// </summary>
    /// <param name="services">The service provider for database access.</param>
    /// <param name="matchId">The match ID this bet is for.</param>
    /// <param name="userId">The user placing the bet.</param>
    /// <param name="prediction">The predicted result (e.g., "2:1" or "X").</param>
    /// <returns>The created bet's ID.</returns>
    public static async Task<Guid> SeedBetAsync(
        IServiceProvider services,
        Guid matchId,
        Guid userId,
        string prediction)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var betId = Guid.NewGuid();
        var editedAt = DateTime.UtcNow;

        await dbContext.Database.ExecuteSqlRawAsync(@"
            INSERT INTO bets (id, match_id, user_id, prediction, edited_at)
            VALUES ({0}, {1}, {2}, {3}, {4})",
            betId, matchId, userId, prediction, editedAt);

        return betId;
    }
}
