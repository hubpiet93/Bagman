using Bagman.Contracts.Models.Tables;
using Bagman.IntegrationTests.Helpers;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Tables;

/// <summary>
///     Helper methods for creating tables in endpoint tests.
///     Each method is generic and can return HttpResponseMessage (for snapshot testing)
///     or a concrete type (for deserialization).
/// </summary>
public static class TableCreationHelpers
{
    private const string CreateTableEndpoint = "/api/tables";
    private const string CreateAuthorizedTableEndpoint = "/api/tables/create";

    /// <summary>
    ///     Creates a table via the legacy /api/tables endpoint (no prior authentication required).
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or TableResponse for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="request">The table creation request containing user credentials and table details.</param>
    /// <param name="token">Optional Bearer token (typically null for this legacy endpoint).</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> CreateTableAsync<T>(
        this HttpClient client,
        CreateTableRequest request,
        string? token = null) where T : class
    {
        return client.PostAsync<T>(CreateTableEndpoint, request, token);
    }

    /// <summary>
    ///     Creates a table via the authorized /api/tables/create endpoint.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or TableResponse for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="request">The authorized table creation request.</param>
    /// <param name="token">Optional Bearer token for authentication. When null, no Authorization header is added.</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> CreateAuthorizedTableAsync<T>(
        this HttpClient client,
        AuthorizedCreateTableRequest request,
        string? token = null) where T : class
    {
        return client.PostAsync<T>(CreateAuthorizedTableEndpoint, request, token);
    }

    #region Request Factory Methods

    /// <summary>
    ///     Creates a default CreateTableRequest with sensible test defaults.
    /// </summary>
    /// <param name="eventTypeId">The event type ID for the table.</param>
    /// <param name="userLogin">Optional user login (default: generates unique).</param>
    /// <param name="tableName">Optional table name (default: generates unique).</param>
    /// <param name="stake">The stake amount (default: 50m).</param>
    /// <param name="maxPlayers">Maximum number of players (default: 10).</param>
    /// <param name="userPassword">Optional user password (default: TestConstants.CreatorPassword).</param>
    /// <returns>A CreateTableRequest with sensible defaults.</returns>
    public static CreateTableRequest CreateDefaultTableRequest(
        Guid eventTypeId,
        string? userLogin = null,
        string? tableName = null,
        decimal stake = 50m,
        int maxPlayers = 10,
        string? userPassword = null)
    {
        return new CreateTableRequest
        {
            UserLogin = userLogin ?? Unique("creator_user"),
            UserPassword = userPassword ?? TestConstants.CreatorPassword,
            TableName = tableName ?? $"Test Betting Table {Guid.NewGuid()}",
            TablePassword = TestConstants.DefaultTablePassword,
            MaxPlayers = maxPlayers,
            Stake = stake,
            EventTypeId = eventTypeId
        };
    }

    /// <summary>
    ///     Creates a default AuthorizedCreateTableRequest with sensible test defaults.
    /// </summary>
    /// <param name="eventTypeId">The event type ID for the table.</param>
    /// <param name="tableName">Optional specific table name. If not provided, generates a unique one.</param>
    /// <param name="stake">The stake amount (default: 25m).</param>
    /// <param name="maxPlayers">Maximum number of players (default: 10).</param>
    /// <returns>An AuthorizedCreateTableRequest with sensible defaults.</returns>
    public static AuthorizedCreateTableRequest CreateDefaultAuthorizedTableRequest(
        Guid eventTypeId,
        string? tableName = null,
        decimal stake = 25m,
        int maxPlayers = 10)
    {
        return new AuthorizedCreateTableRequest
        {
            TableName = tableName ?? $"Authorized Table {Guid.NewGuid()}",
            TablePassword = TestConstants.AuthTablePassword,
            MaxPlayers = maxPlayers,
            Stake = stake,
            EventTypeId = eventTypeId
        };
    }

    private static string Unique(string prefix) =>
        AuthEndpointsHelpers.Unique(prefix);

    #endregion
}
