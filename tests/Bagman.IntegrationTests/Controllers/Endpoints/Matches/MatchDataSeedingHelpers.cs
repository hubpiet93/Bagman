using Bagman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Matches;

/// <summary>
///     Helper methods for seeding Match data directly into the database.
///     Used for testing scenarios that require matches with past dates or specific results.
/// </summary>
public static class MatchDataSeedingHelpers
{
    /// <summary>
    ///     Seeds a Match record directly into the database using raw SQL.
    ///     This bypasses domain validation (e.g., future date requirement).
    /// </summary>
    /// <param name="services">The service provider for database access.</param>
    /// <param name="eventTypeId">The event type ID this match belongs to.</param>
    /// <param name="country1">First country/team name.</param>
    /// <param name="country2">Second country/team name.</param>
    /// <param name="matchDateTime">The match date/time (can be in the past for testing).</param>
    /// <param name="status">Match status: "scheduled" or "finished".</param>
    /// <param name="result">Optional match result (e.g., "2:1").</param>
    /// <returns>The created match's ID.</returns>
    public static async Task<Guid> SeedMatchAsync(
        IServiceProvider services,
        Guid eventTypeId,
        string country1,
        string country2,
        DateTime matchDateTime,
        string status = "scheduled",
        string? result = null)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var matchId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        await dbContext.Database.ExecuteSqlRawAsync(@"
            INSERT INTO matches (id, event_type_id, country_1, country_2, match_datetime, status, result, created_at)
            VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})",
            matchId, eventTypeId, country1, country2, matchDateTime, status, result, createdAt);

        return matchId;
    }

    /// <summary>
    ///     Seeds a finished Match with result directly into the database.
    ///     Convenience method for tests that need a completed match.
    /// </summary>
    /// <param name="services">The service provider for database access.</param>
    /// <param name="eventTypeId">The event type ID this match belongs to.</param>
    /// <param name="country1">First country/team name.</param>
    /// <param name="country2">Second country/team name.</param>
    /// <param name="result">The match result (e.g., "2:1").</param>
    /// <param name="matchDateTime">Optional match date/time (defaults to 30 minutes ago).</param>
    /// <returns>The created match's ID.</returns>
    public static Task<Guid> SeedFinishedMatchAsync(
        IServiceProvider services,
        Guid eventTypeId,
        string country1,
        string country2,
        string result,
        DateTime? matchDateTime = null)
    {
        return SeedMatchAsync(
            services,
            eventTypeId,
            country1,
            country2,
            matchDateTime ?? DateTime.UtcNow.AddMinutes(-30),
            "finished",
            result);
    }
}
