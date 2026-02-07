using Bagman.Contracts.Models.Tables;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Tables;

/// <summary>
///     Helper methods for managing table membership and admin rights.
/// </summary>
public static class TableMembershipHelpers
{
    /// <summary>
    ///     Leaves a table as the specified user.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table to leave.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>The response from the leave operation.</returns>
    public static async Task<dynamic> LeaveTableAsync(this HttpClient client, Guid tableId, string token)
    {
        return await client.DeleteAsJsonAsync<dynamic>($"/api/tables/{tableId}/members", token);
    }

    /// <summary>
    ///     Grants admin rights to a user in a specific table.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="userId">The ID of the user to promote.</param>
    /// <param name="token">The Bearer token for authentication (must be table creator or admin).</param>
    /// <returns>The response from the grant operation.</returns>
    public static async Task<dynamic> GrantAdminAsync(this HttpClient client, Guid tableId, Guid userId, string token)
    {
        var request = new GrantAdminRequest { UserId = userId };
        return await client.PostAsJsonAsync<dynamic>($"/api/tables/{tableId}/admins", request, token);
    }

    /// <summary>
    ///     Revokes admin rights from a user in a specific table.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="userId">The ID of the user to demote.</param>
    /// <param name="token">The Bearer token for authentication (must be table creator or admin).</param>
    /// <returns>The response from the revoke operation.</returns>
    public static async Task<dynamic> RevokeAdminAsync(this HttpClient client, Guid tableId, Guid userId, string token)
    {
        return await client.DeleteAsJsonAsync<dynamic>($"/api/tables/{tableId}/admins/{userId}", token);
    }
}
