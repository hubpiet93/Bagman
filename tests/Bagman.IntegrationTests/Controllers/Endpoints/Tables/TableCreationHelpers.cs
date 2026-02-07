using Bagman.Contracts.Models.Tables;
using Bagman.IntegrationTests.Helpers;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Tables;

/// <summary>
///     Helper methods for creating tables in endpoint tests.
///     Supports both unauthorized (legacy /api/tables) and authorized (new /api/tables/create) flows.
/// </summary>
public static class TableCreationHelpers
{
    /// <summary>
    ///     Creates a table with the given parameters (no user registration required).
    ///     For snapshot testing - doesn't deserialize the response.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="eventTypeId">The event type ID for the table.</param>
    /// <param name="userLogin">Optional user login (default: generates unique).</param>
    /// <param name="tableName">Optional table name (default: generates unique).</param>
    /// <param name="stake">The stake amount (default: 50m).</param>
    /// <param name="maxPlayers">Maximum number of players (default: 10).</param>
    /// <returns>HttpResponseMessage from the create table endpoint.</returns>
    public static async Task<HttpResponseMessage> CreateTableNoRegisterAsync(
        this HttpClient client,
        Guid eventTypeId,
        string? userLogin = null,
        string? tableName = null,
        decimal stake = 50m,
        int maxPlayers = 10)
    {
        var request = new CreateTableRequest
        {
            UserLogin = userLogin ?? Unique("creator_user"),
            UserPassword = TestConstants.CreatorPassword,
            TableName = tableName ?? $"Test Betting Table {Guid.NewGuid()}",
            TablePassword = TestConstants.DefaultTablePassword,
            MaxPlayers = maxPlayers,
            Stake = stake,
            EventTypeId = eventTypeId
        };

        return await client.PostAsJsonWithoutDeserializeAsync("/api/tables", request);
    }

    /// <summary>
    ///     Creates a table with an invalid field for validation testing.
    ///     For snapshot testing - doesn't deserialize the response.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="eventTypeId">The event type ID for the table.</param>
    /// <param name="validationType">The validation error to trigger: "empty_name", "negative_stake", "no_players".</param>
    /// <returns>HttpResponseMessage from the create table endpoint.</returns>
    public static async Task<HttpResponseMessage> CreateTableInvalidAsync(
        this HttpClient client,
        Guid eventTypeId,
        string validationType)
    {
        var request = validationType switch
        {
            "empty_name" => new CreateTableRequest
            {
                UserLogin = Unique("user_invalid_table"),
                UserPassword = TestConstants.DefaultUserPassword,
                TableName = "",
                TablePassword = TestConstants.DefaultTablePassword,
                MaxPlayers = TestConstants.DefaultMaxPlayers,
                Stake = TestConstants.DefaultStake,
                EventTypeId = eventTypeId
            },
            "negative_stake" => new CreateTableRequest
            {
                UserLogin = Unique("user_negative_stake"),
                UserPassword = TestConstants.DefaultUserPassword,
                TableName = $"Test Table {Guid.NewGuid()}",
                TablePassword = TestConstants.DefaultTablePassword,
                MaxPlayers = TestConstants.DefaultMaxPlayers,
                Stake = -TestConstants.DefaultStake,
                EventTypeId = eventTypeId
            },
            "no_players" => new CreateTableRequest
            {
                UserLogin = Unique("user_no_players"),
                UserPassword = TestConstants.DefaultUserPassword,
                TableName = $"Test Table {Guid.NewGuid()}",
                TablePassword = TestConstants.DefaultTablePassword,
                MaxPlayers = 0,
                Stake = TestConstants.DefaultStake,
                EventTypeId = eventTypeId
            },
            _ => throw new ArgumentException($"Unknown validation type: {validationType}")
        };

        return await client.PostAsJsonWithoutDeserializeAsync("/api/tables", request);
    }

    /// <summary>
    ///     Creates a table and returns the created table response with all relevant details.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="eventTypeId">The event type ID for the table.</param>
    /// <param name="userLogin">Optional user login (default: generates unique).</param>
    /// <param name="tableName">Optional table name (default: generates unique).</param>
    /// <param name="stake">The stake amount (default: 50m).</param>
    /// <param name="maxPlayers">Maximum number of players (default: 10).</param>
    /// <returns>The created TableResponse containing the table ID and details.</returns>
    public static async Task<TableResponse> CreateTableAsync(
        this HttpClient client,
        Guid eventTypeId,
        string? userLogin = null,
        string? tableName = null,
        decimal stake = 50m,
        int maxPlayers = 10)
    {
        var request = new CreateTableRequest
        {
            UserLogin = userLogin ?? Unique("creator_user"),
            UserPassword = TestConstants.CreatorPassword,
            TableName = tableName ?? $"Test Betting Table {Guid.NewGuid()}",
            TablePassword = TestConstants.DefaultTablePassword,
            MaxPlayers = maxPlayers,
            Stake = stake,
            EventTypeId = eventTypeId
        };

        return await client.PostAsJsonAsync<TableResponse>("/api/tables", request);
    }

    /// <summary>
    ///     Creates an authorized table creation request with default values.
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

    /// <summary>
    ///     Creates an invalid authorized table creation request for validation testing.
    /// </summary>
    /// <param name="eventTypeId">The event type ID for the table.</param>
    /// <param name="validationType">The validation error to trigger: "empty_name", "negative_stake", "no_players".</param>
    /// <returns>An AuthorizedCreateTableRequest with specific validation issues.</returns>
    public static AuthorizedCreateTableRequest CreateInvalidAuthorizedTableRequest(Guid eventTypeId, string validationType)
    {
        return validationType switch
        {
            "empty_name" => new AuthorizedCreateTableRequest
            {
                TableName = "",
                TablePassword = TestConstants.AuthTablePassword,
                MaxPlayers = TestConstants.DefaultMaxPlayers,
                Stake = TestConstants.AuthTableStake,
                EventTypeId = eventTypeId
            },
            "negative_stake" => new AuthorizedCreateTableRequest
            {
                TableName = $"Invalid Table {Guid.NewGuid()}",
                TablePassword = TestConstants.AuthTablePassword,
                MaxPlayers = TestConstants.DefaultMaxPlayers,
                Stake = -TestConstants.AuthTableStake,
                EventTypeId = eventTypeId
            },
            "no_players" => new AuthorizedCreateTableRequest
            {
                TableName = $"Invalid Table {Guid.NewGuid()}",
                TablePassword = TestConstants.AuthTablePassword,
                MaxPlayers = 0,
                Stake = TestConstants.AuthTableStake,
                EventTypeId = eventTypeId
            },
            _ => throw new ArgumentException($"Unknown validation type: {validationType}")
        };
    }

    /// <summary>
    ///     Creates an authorized table via the /api/tables/create endpoint with a bearer token.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="request">The AuthorizedCreateTableRequest to use.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>The created TableResponse containing the table ID and details.</returns>
    public static async Task<TableResponse> CreateAuthorizedTableAsync(
        this HttpClient client,
        AuthorizedCreateTableRequest request,
        string token)
    {
        return await client.PostAsJsonAsync<TableResponse>("/api/tables/create", request, token);
    }
    
    /// <summary>
    ///     Creates an authorized table via the /api/tables/create endpoint with a bearer token.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="eventTypeId">The event type ID for the table.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>The created TableResponse containing the table ID and details.</returns>
    public static async Task<TableResponse> CreateAuthorizedTableAsync(
        this HttpClient client, Guid eventTypeId,
        string token)
    {
        var request = new AuthorizedCreateTableRequest
        {
            TableName = $"Authorized Table {Guid.NewGuid()}",
            TablePassword = TestConstants.AuthTablePassword,
            MaxPlayers = TestConstants.DefaultMaxPlayers,
            Stake = TestConstants.AuthTableStake,
            EventTypeId = eventTypeId
        };
        
        return await client.PostAsJsonAsync<TableResponse>("/api/tables/create", request, token);
    }

    /// <summary>
    ///     Creates an invalid authorized table via the /api/tables/create endpoint for validation testing.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="eventTypeId">The event type ID for the table.</param>
    /// <param name="validationType">The validation error to trigger: "empty_name", "negative_stake", "no_players".</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>Dynamic response from the endpoint.</returns>
    public static async Task<dynamic> CreateAuthorizedTableInvalidAsync(
        this HttpClient client,
        Guid eventTypeId,
        string validationType,
        string token)
    {
        var request = CreateInvalidAuthorizedTableRequest(eventTypeId, validationType);
        return await client.PostAsJsonAsync<dynamic>("/api/tables/create", request, token);
    }

    /// <summary>
    ///     Creates an authorized table without a token (for testing unauthorized requests).
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="request">The authorized table creation request.</param>
    /// <returns>HttpResponseMessage from the endpoint.</returns>
    public static async Task<HttpResponseMessage> CreateAuthorizedTableNoTokenAsync(
        this HttpClient client,
        AuthorizedCreateTableRequest request)
    {
        return await client.PostAsJsonWithoutDeserializeAsync("/api/tables/create", request);
    }

    /// <summary>
    ///     Generates a unique string by delegating to AuthEndpointsHelpers.
    ///     Useful for creating unique logins and table names.
    /// </summary>
    /// <param name="prefix">The prefix for the unique string.</param>
    /// <returns>A unique string in format "prefix_guidshortened".</returns>
    private static string Unique(string prefix)
    {
        return Controllers.Endpoints.AuthEndpointsHelpers.Unique(prefix);
    }
}
