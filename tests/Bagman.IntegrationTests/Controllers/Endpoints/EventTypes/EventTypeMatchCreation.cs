using Bagman.Contracts.Models.Tables;

namespace Bagman.IntegrationTests.Controllers.Endpoints.EventTypes;

public static class EventTypeMatchCreation
{
    /// <summary>
    ///     Creates a match as a super admin user.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="eventTypeId">The ID of the event type.</param>
    /// <param name="country1">First country/team name.</param>
    /// <param name="country2">Second country/team name.</param>
    /// <param name="superAdminToken">The Bearer token of a super admin user.</param>
    /// <param name="matchDateTime">Optional match date/time (default: tomorrow).</param>
    /// <returns>The created MatchResponse containing the match ID and details.</returns>
    public static async Task<MatchResponse> CreateMatchAsync(
        this HttpClient client,
        Guid eventTypeId,
        string country1,
        string country2,
        string superAdminToken,
        DateTime? matchDateTime = null)
    {
        var request = new CreateMatchRequest
        {
            Country1 = country1,
            Country2 = country2,
            MatchDateTime = matchDateTime ?? DateTime.UtcNow.AddDays(1)
        };

        return await client.PostAsJsonAsync<MatchResponse>(
            $"/api/admin/event-types/{eventTypeId}/matches",
            request,
            superAdminToken);
    }
}
