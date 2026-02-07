using System.Net.Http.Headers;
using Bagman.Contracts.Models.Tables;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Matches;

/// <summary>
///     Helper methods for querying match data in endpoint tests.
/// </summary>
public static class MatchQueryHelpers
{
    /// <summary>
    ///     Gets match details with Started flag calculated based on current time.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="matchId">The ID of the match.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>The match response with details.</returns>
    public static async Task<MatchResponse> GetMatchAsync(
        this HttpClient client,
        Guid tableId,
        Guid matchId,
        string token)
    {
        return await client.GetAsJsonAsync<MatchResponse>(
            $"/api/tables/{tableId}/matches/{matchId}",
            token);
    }

    /// <summary>
    ///     Gets match details for a table (for snapshot testing).
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="matchId">The ID of the match.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>HttpResponseMessage for snapshot testing.</returns>
    public static async Task<HttpResponseMessage> GetMatchForTableAsync(
        this HttpClient client,
        Guid tableId,
        Guid matchId,
        string token)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/tables/{tableId}/matches/{matchId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(request);
    }
}
