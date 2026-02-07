using Bagman.Contracts.Models.Tables;

namespace Bagman.IntegrationTests.Controllers.Endpoints.EventTypes;

/// <summary>
///     Helper methods for creating matches in endpoint tests.
///     Each method is generic and can return HttpResponseMessage (for snapshot testing)
///     or a concrete type (for deserialization).
/// </summary>
public static class EventTypeMatchCreation
{
    /// <summary>
    ///     Creates a match for an event type.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or MatchResponse for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="eventTypeId">The ID of the event type.</param>
    /// <param name="request">The create match request.</param>
    /// <param name="token">Optional Bearer token of a super admin user.</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> CreateMatchAsync<T>(
        this HttpClient client,
        Guid eventTypeId,
        CreateMatchRequest request,
        string? token = null) where T : class
    {
        return client.PostAsync<T>($"/api/admin/event-types/{eventTypeId}/matches", request, token);
    }

    #region Request Factory Methods

    /// <summary>
    ///     Creates a default CreateMatchRequest with sensible test defaults.
    /// </summary>
    /// <param name="country1">First country/team name.</param>
    /// <param name="country2">Second country/team name.</param>
    /// <param name="matchDateTime">Optional match date/time (default: tomorrow).</param>
    /// <returns>A CreateMatchRequest with sensible defaults.</returns>
    public static CreateMatchRequest CreateMatchRequest(
        string country1,
        string country2,
        DateTime? matchDateTime = null)
    {
        return new CreateMatchRequest
        {
            Country1 = country1,
            Country2 = country2,
            MatchDateTime = matchDateTime ?? DateTime.UtcNow.AddDays(1)
        };
    }

    #endregion
}
