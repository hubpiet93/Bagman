using System.Net.Http.Headers;
using Bagman.Contracts.Models.Tables;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Bets;

public static class BetPlacing
{
    /// <summary>
    ///     Places a bet on a specific match in a table (for snapshot testing).
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="matchId">The ID of the match to bet on.</param>
    /// <param name="prediction">The prediction for the bet.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>HttpResponseMessage for snapshot testing.</returns>
    public static async Task<HttpResponseMessage> PlaceBetForTableAsync(
        this HttpClient client,
        Guid tableId,
        Guid matchId,
        string prediction,
        string token)
    {
        var request = new PlaceBetRequest { Prediction = prediction };
        return await client.PostAsJsonWithoutDeserializeAsync(
            $"/api/tables/{tableId}/matches/{matchId}/bets",
            request,
            token);
    }
    /// <summary>
    ///     Places a bet on a specific match in a table.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="matchId">The ID of the match to bet on.</param>
    /// <param name="prediction">The prediction for the bet.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>The response from the bet placement operation.</returns>
    public static async Task<dynamic> PlaceBetAsync(
        this HttpClient client,
        Guid tableId,
        Guid matchId,
        string prediction,
        string token)
    {
        var request = new PlaceBetRequest { Prediction = prediction };
        return await client.PostAsJsonAsync<dynamic>(
            $"/api/tables/{tableId}/matches/{matchId}/bets",
            request,
            token);
    }

    /// <summary>
    ///     Gets a specific bet placed by the user on a match (for snapshot testing).
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="matchId">The ID of the match.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>HttpResponseMessage for snapshot testing.</returns>
    public static async Task<HttpResponseMessage> GetUserBetForTableAsync(
        this HttpClient client,
        Guid tableId,
        Guid matchId,
        string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/tables/{tableId}/matches/{matchId}/bets/my");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(request);
    }

    /// <summary>
    ///     Gets a specific bet placed by the user on a match.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="matchId">The ID of the match.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>The bet details.</returns>
    public static async Task<dynamic> GetUserBetAsync(
        this HttpClient client,
        Guid tableId,
        Guid matchId,
        string token)
    {
        return await client.GetAsJsonAsync<dynamic>(
            $"/api/tables/{tableId}/matches/{matchId}/bets",
            token);
    }

    /// <summary>
    ///     Deletes a bet placed by the user on a match.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="matchId">The ID of the match.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>The response from the delete operation.</returns>
    public static async Task<dynamic> DeleteBetAsync(
        this HttpClient client,
        Guid tableId,
        Guid matchId,
        string token)
    {
        return await client.DeleteAsJsonAsync<dynamic>(
            $"/api/tables/{tableId}/matches/{matchId}/bets",
            token);
    }

    /// <summary>
    ///     Deletes a bet placed by the user on a match (for snapshot testing).
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="matchId">The ID of the match.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>HttpResponseMessage for snapshot testing.</returns>
    public static async Task<HttpResponseMessage> DeleteBetForTableAsync(
        this HttpClient client,
        Guid tableId,
        Guid matchId,
        string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/tables/{tableId}/matches/{matchId}/bets");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(request);
    }
}
