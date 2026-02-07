using Bagman.Contracts.Models.Tables;
using Bagman.IntegrationTests.Helpers;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Tables;

/// <summary>
///     Helper methods for joining tables in endpoint tests.
///     Each method is generic and can return HttpResponseMessage (for snapshot testing)
///     or a concrete type (for deserialization).
/// </summary>
public static class TableJoinHelpers
{
    private const string LegacyJoinEndpoint = "/api/tables/join";
    private const string AuthorizedJoinEndpoint = "/api/tables/{0}/join";

    /// <summary>
    ///     Joins a table via the legacy /api/tables/join endpoint.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="request">The join table request containing user credentials and table details.</param>
    /// <param name="token">Optional Bearer token (typically null for this legacy endpoint).</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> JoinTableAsync<T>(
        this HttpClient client,
        JoinTableRequest request,
        string? token = null) where T : class
    {
        return client.PostAsync<T>(LegacyJoinEndpoint, request, token);
    }

    /// <summary>
    ///     Joins a table via the authorized /api/tables/{tableId}/join endpoint.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table to join.</param>
    /// <param name="request">The authorized join request containing the table password.</param>
    /// <param name="token">Optional Bearer token for authentication. When null, no Authorization header is added.</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> JoinTableAuthorizedAsync<T>(
        this HttpClient client,
        Guid tableId,
        JoinTableAuthorizedRequest request,
        string? token = null) where T : class
    {
        var endpoint = string.Format(AuthorizedJoinEndpoint, tableId);
        return client.PostAsync<T>(endpoint, request, token);
    }

    #region Request Factory Methods

    /// <summary>
    ///     Creates a default JoinTableRequest with sensible test defaults.
    /// </summary>
    /// <param name="tableName">The name of the table to join.</param>
    /// <param name="tablePassword">The password of the table.</param>
    /// <param name="userLogin">Optional user login (default: generates unique).</param>
    /// <param name="userPassword">Optional user password (default: TestConstants.JoinerPassword).</param>
    /// <returns>A JoinTableRequest with sensible defaults.</returns>
    public static JoinTableRequest CreateJoinTableRequest(
        string tableName,
        string tablePassword,
        string? userLogin = null,
        string? userPassword = null)
    {
        return new JoinTableRequest
        {
            UserLogin = userLogin ?? Unique("joining_user"),
            UserPassword = userPassword ?? TestConstants.JoinerPassword,
            TableName = tableName,
            TablePassword = tablePassword
        };
    }

    /// <summary>
    ///     Creates a JoinTableAuthorizedRequest.
    /// </summary>
    /// <param name="password">The table password.</param>
    /// <returns>A JoinTableAuthorizedRequest.</returns>
    public static JoinTableAuthorizedRequest CreateJoinTableAuthorizedRequest(string password)
    {
        return new JoinTableAuthorizedRequest { Password = password };
    }

    private static string Unique(string prefix) =>
        AuthEndpointsHelpers.Unique(prefix);

    #endregion

    #region Scenario Helpers

    /// <summary>
    ///     Joins a table as an already-registered user.
    ///     This is a convenience method that wraps JoinTableAsync with an existing user's credentials.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableName">The name of the table to join.</param>
    /// <param name="tablePassword">The password of the table.</param>
    /// <param name="userLogin">The login of the already-registered user.</param>
    /// <param name="userPassword">The password of the already-registered user.</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> JoinTableAsExistingUserAsync<T>(
        this HttpClient client,
        string tableName,
        string tablePassword,
        string userLogin,
        string userPassword) where T : class
    {
        var request = new JoinTableRequest
        {
            UserLogin = userLogin,
            UserPassword = userPassword,
            TableName = tableName,
            TablePassword = tablePassword
        };
        return client.JoinTableAsync<T>(request);
    }

    /// <summary>
    ///     Joins a table with a new user and returns the user token, user ID, and login.
    ///     This is a multi-step scenario helper, not a simple endpoint wrapper.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableName">The name of the table to join.</param>
    /// <param name="tablePassword">The password of the table.</param>
    /// <param name="userPrefix">Prefix for generating unique user login (default: "joining_user").</param>
    /// <returns>Tuple containing (token, userId, login).</returns>
    public static async Task<(string Token, Guid UserId, string Login)> JoinTableAsNewUserAsync(
        this HttpClient client,
        string tableName,
        string tablePassword,
        string userPrefix = "joining_user")
    {
        var (token, userId, login) = await client.RegisterAndGetTokenAsync(userPrefix, TestConstants.DefaultUserPassword);

        var joinRequest = CreateJoinTableRequest(tableName, tablePassword, login, TestConstants.DefaultUserPassword);
        await client.JoinTableAsync<dynamic>(joinRequest);

        return (token, userId, login);
    }

    #endregion
}
