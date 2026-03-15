using Bagman.Contracts.Models.Tables;
using Bagman.IntegrationTests.Controllers.Endpoints;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Tables;

/// <summary>
///     Helper methods for managing table membership and admin rights.
///     Each method is generic and can return HttpResponseMessage (for snapshot testing)
///     or a concrete type (for deserialization).
/// </summary>
public static class TableMembershipHelpers
{
    /// <summary>
    ///     Leaves a table as the specified user.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table to leave.</param>
    /// <param name="token">Optional Bearer token for authentication.</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> LeaveTableAsync<T>(
        this HttpClient client,
        Guid tableId,
        string? token = null) where T : class
    {
        return client.DeleteAsync<T>($"/api/tables/{tableId}/members", token);
    }

    /// <summary>
    ///     Grants admin rights to a user in a specific table.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="request">The grant admin request containing the user ID to promote.</param>
    /// <param name="token">Optional Bearer token for authentication (must be table creator or admin).</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> GrantAdminAsync<T>(
        this HttpClient client,
        Guid tableId,
        GrantAdminRequest request,
        string? token = null) where T : class
    {
        return client.PostAsync<T>($"/api/tables/{tableId}/admins", request, token);
    }

    /// <summary>
    ///     Revokes admin rights from a user in a specific table.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="userId">The ID of the user to demote.</param>
    /// <param name="token">Optional Bearer token for authentication (must be table creator or admin).</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> RevokeAdminAsync<T>(
        this HttpClient client,
        Guid tableId,
        Guid userId,
        string? token = null) where T : class
    {
        return client.DeleteAsync<T>($"/api/tables/{tableId}/admins/{userId}", token);
    }

    /// <summary>
    ///     Updates table parameters via PATCH /api/tables/{tableId}.
    ///     Only provided fields are updated (partial update). Requires a table admin token.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table to update.</param>
    /// <param name="request">The update request (all fields optional).</param>
    /// <param name="token">Optional Bearer token for authentication (must be table admin).</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> UpdateTableAsync<T>(
        this HttpClient client,
        Guid tableId,
        UpdateTableRequest request,
        string? token = null) where T : class
    {
        return client.PatchAsync<T>($"/api/tables/{tableId}", request, token);
    }

    /// <summary>
    ///     Deletes a table. Only the table creator can delete it.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table to delete.</param>
    /// <param name="token">Optional Bearer token for authentication.</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> DeleteTableAsync<T>(
        this HttpClient client,
        Guid tableId,
        string? token = null) where T : class
    {
        return client.DeleteAsync<T>($"/api/tables/{tableId}", token);
    }

    /// <summary>
    ///     Kicks a member from a table. Requires admin rights.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="userId">The ID of the user to kick.</param>
    /// <param name="token">Optional Bearer token for authentication (must be table admin).</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> KickMemberAsync<T>(
        this HttpClient client,
        Guid tableId,
        Guid userId,
        string? token = null) where T : class
    {
        return client.DeleteAsync<T>($"/api/tables/{tableId}/members/{userId}", token);
    }
}
