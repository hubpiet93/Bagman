using Bagman.IntegrationTests.Helpers;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Tables;

/// <summary>
///     Helper methods for complex, multi-step table scenarios in endpoint tests.
/// </summary>
public static class TableScenarioHelpers
{
    /// <summary>
    ///     Creates a table as a registered user and returns table ID, user token, and user login.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="eventTypeId">The event type ID for the table.</param>
    /// <param name="userPrefix">Prefix for generating unique login (default: "table_creator").</param>
    /// <param name="tableName">Optional specific table name. If not provided, generates a unique one.</param>
    /// <returns>Tuple containing (tableId, token, login).</returns>
    public static async Task<(Guid TableId, string Token, string Login)> CreateTableAsync(
        this HttpClient client,
        Guid eventTypeId,
        string userPrefix = "table_creator",
        string? tableName = null)
    {
        var (token, _, login) = await client.RegisterAndGetTokenAsync(userPrefix, TestConstants.DefaultUserPassword);

        var table = await client.CreateTableAsync(
            eventTypeId,
            userLogin: login,
            tableName: tableName);

        return (table.Id, token, login);
    }
}
