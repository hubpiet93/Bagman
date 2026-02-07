using Bagman.Contracts.Models.Tables;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Bets;

/// <summary>
///     Helper methods for bet operations in endpoint tests.
///     Each method is generic and can return HttpResponseMessage (for snapshot testing)
///     or a concrete type (for deserialization).
/// </summary>
public static class BetPlacing
{
    private const string BetEndpoint = "/api/tables/{0}/matches/{1}/bets";
    private const string MyBetEndpoint = "/api/tables/{0}/matches/{1}/bets/my";

    /// <summary>
    ///     Places a bet on a specific match in a table.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="matchId">The ID of the match to bet on.</param>
    /// <param name="request">The bet placement request containing the prediction.</param>
    /// <param name="token">Optional Bearer token for authentication. When null, no Authorization header is added.</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> PlaceBetAsync<T>(
        this HttpClient client,
        Guid tableId,
        Guid matchId,
        PlaceBetRequest request,
        string? token = null) where T : class
    {
        var endpoint = string.Format(BetEndpoint, tableId, matchId);
        return client.PostAsync<T>(endpoint, request, token);
    }

    /// <summary>
    ///     Gets a specific bet placed by the user on a match.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="matchId">The ID of the match.</param>
    /// <param name="token">Optional Bearer token for authentication.</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> GetUserBetAsync<T>(
        this HttpClient client,
        Guid tableId,
        Guid matchId,
        string? token = null) where T : class
    {
        var endpoint = string.Format(MyBetEndpoint, tableId, matchId);
        return client.GetAsync<T>(endpoint, token);
    }

    /// <summary>
    ///     Deletes a bet placed by the user on a match.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="matchId">The ID of the match.</param>
    /// <param name="token">Optional Bearer token for authentication.</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> DeleteBetAsync<T>(
        this HttpClient client,
        Guid tableId,
        Guid matchId,
        string? token = null) where T : class
    {
        var endpoint = string.Format(BetEndpoint, tableId, matchId);
        return client.DeleteAsync<T>(endpoint, token);
    }
}
