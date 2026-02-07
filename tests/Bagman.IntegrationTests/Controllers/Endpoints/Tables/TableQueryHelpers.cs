using Bagman.Contracts.Models.Tables;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Tables;

/// <summary>
///     Helper methods for querying table data in endpoint tests.
/// </summary>
public static class TableQueryHelpers
{
    /// <summary>
    ///     Gets the list of tables for the authenticated user.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>The list of user tables.</returns>
    public static async Task<dynamic> GetUserTablesAsync(this HttpClient client, string token)
    {
        return await client.GetAsJsonAsync<dynamic>("/api/tables", token);
    }

    /// <summary>
    ///     Gets the details of a specific table with all member information.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>The table response containing table details and member list.</returns>
    public static async Task<TableResponse> GetTableDetailsAsync(this HttpClient client, Guid tableId, string token)
    {
        return await client.GetAsJsonAsync<TableResponse>($"/api/tables/{tableId}", token);
    }

    /// <summary>
    ///     Gets the table dashboard for the authenticated user.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>The table dashboard containing table info, members, matches, and bets.</returns>
    public static async Task<dynamic> GetTableDashboardAsync(this HttpClient client, Guid tableId, string token)
    {
        return await client.GetAsJsonAsync<dynamic>($"/api/tables/{tableId}/dashboard", token);
    }

    /// <summary>
    ///     Gets the table dashboard without authentication token (for testing unauthorized requests).
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <returns>HttpResponseMessage from the endpoint.</returns>
    public static async Task<HttpResponseMessage> GetTableDashboardWithoutTokenAsync(
        this HttpClient client,
        Guid tableId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/tables/{tableId}/dashboard");
        return await client.SendAsync(request);
    }
}
