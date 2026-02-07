using Bagman.Contracts.Models.Tables;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Tables;

/// <summary>
///     Helper methods for querying table data in endpoint tests.
///     Each method is generic and can return HttpResponseMessage (for snapshot testing)
///     or a concrete type (for deserialization).
/// </summary>
public static class TableQueryHelpers
{
    /// <summary>
    ///     Gets the list of tables for the authenticated user.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="token">Optional Bearer token for authentication.</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> GetUserTablesAsync<T>(
        this HttpClient client,
        string? token = null) where T : class
    {
        return client.GetAsync<T>("/api/tables", token);
    }

    /// <summary>
    ///     Gets the details of a specific table with all member information.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or TableResponse for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="token">Optional Bearer token for authentication.</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> GetTableDetailsAsync<T>(
        this HttpClient client,
        Guid tableId,
        string? token = null) where T : class
    {
        return client.GetAsync<T>($"/api/tables/{tableId}", token);
    }

    /// <summary>
    ///     Gets the table dashboard for the authenticated user.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="token">Optional Bearer token for authentication.</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> GetTableDashboardAsync<T>(
        this HttpClient client,
        Guid tableId,
        string? token = null) where T : class
    {
        return client.GetAsync<T>($"/api/tables/{tableId}/dashboard", token);
    }
}
