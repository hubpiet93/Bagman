using Bagman.Contracts.Models.Tables;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Matches;

/// <summary>
///     Helper methods for querying match data in endpoint tests.
///     Each method is generic and can return HttpResponseMessage (for snapshot testing)
///     or a concrete type (for deserialization).
/// </summary>
public static class MatchQueryHelpers
{
    /// <summary>
    ///     Gets match details with Started flag calculated based on current time.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or MatchResponse for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="matchId">The ID of the match.</param>
    /// <param name="token">Optional Bearer token for authentication.</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> GetMatchAsync<T>(
        this HttpClient client,
        Guid tableId,
        Guid matchId,
        string? token = null) where T : class
    {
        return client.GetAsync<T>($"/api/tables/{tableId}/matches/{matchId}", token);
    }
}
