using Bagman.Contracts.Models.Tables;
using Bagman.IntegrationTests.Controllers.Endpoints;
using Bagman.IntegrationTests.Helpers;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Tables;

/// <summary>
///     Helper methods for joining tables in endpoint tests.
/// </summary>
public static class TableJoinHelpers
{
    /// <summary>
    ///     Joins a table with a new user (no registration required).
    ///     For snapshot testing - doesn't deserialize the response.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableName">The name of the table to join.</param>
    /// <param name="tablePassword">The password of the table.</param>
    /// <param name="userLogin">Optional user login (default: generates unique).</param>
    /// <returns>HttpResponseMessage from the join endpoint.</returns>
    public static async Task<HttpResponseMessage> JoinTableNoRegisterAsync(
        this HttpClient client,
        string tableName,
        string tablePassword,
        string? userLogin = null)
    {
        var request = new JoinTableRequest
        {
            UserLogin = userLogin ?? Unique("joining_user"),
            UserPassword = TestConstants.JoinerPassword,
            TableName = tableName,
            TablePassword = tablePassword
        };

        return await client.PostAsJsonWithoutDeserializeAsync("/api/tables/join", request);
    }

    /// <summary>
    ///     Joins a table with wrong password (for validation testing).
    ///     For snapshot testing - doesn't deserialize the response.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableName">The name of the table to join.</param>
    /// <param name="password">The correct table password (used to confirm it's wrong).</param>
    /// <returns>HttpResponseMessage from the join endpoint.</returns>
    public static async Task<HttpResponseMessage> JoinTableAsync(
        this HttpClient client,
        string tableName,
        string password)
    {
        var request = new JoinTableRequest
        {
            UserLogin = Unique("joiner_wrong_pass"),
            UserPassword = TestConstants.JoinerPassword,
            TableName = tableName,
            TablePassword = TestConstants.WrongPassword // Intentionally wrong
        };

        return await client.PostAsJsonWithoutDeserializeAsync("/api/tables/join", request);
    }

    /// <summary>
    ///     Joins a table as an already-registered user.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableName">The name of the table to join.</param>
    /// <param name="tablePassword">The password of the table.</param>
    /// <param name="userLogin">The login of the already-registered user.</param>
    /// <param name="userPassword">The password of the already-registered user.</param>
    /// <returns>HttpResponseMessage from the join endpoint.</returns>
    public static async Task<HttpResponseMessage> JoinTableAsExistingUserAsync(
        this HttpClient client,
        string tableName,
        string tablePassword,
        string userLogin,
        string userPassword)
    {
        var request = new JoinTableRequest
        {
            UserLogin = userLogin,
            UserPassword = userPassword,
            TableName = tableName,
            TablePassword = tablePassword
        };

        return await client.PostAsJsonWithoutDeserializeAsync("/api/tables/join", request);
    }

    /// <summary>
    ///     Joins a table with a new user and returns the user token, user ID, and login.
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

        var joinRequest = new JoinTableRequest
        {
            UserLogin = login,
            UserPassword = TestConstants.DefaultUserPassword,
            TableName = tableName,
            TablePassword = tablePassword
        };

        await client.PostAsJsonAsync<dynamic>("/api/tables/join", joinRequest);

        return (token, userId, login);
    }

    /// <summary>
    ///     Generates a unique string by delegating to AuthEndpointsHelpers.
    ///     Useful for creating unique logins.
    /// </summary>
    /// <param name="prefix">The prefix for the unique string.</param>
    /// <returns>A unique string in format "prefix_guidshortened".</returns>
    private static string Unique(string prefix)
    {
        return AuthEndpointsHelpers.Unique(prefix);
    }

    /// <summary>
    ///     Joins a table as an authenticated user using the authorized endpoint.
    ///     For snapshot testing - doesn't deserialize the response.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table to join.</param>
    /// <param name="tablePassword">The password of the table.</param>
    /// <param name="token">The authentication token of the user.</param>
    /// <returns>HttpResponseMessage from the authorized join endpoint.</returns>
    public static async Task<HttpResponseMessage> JoinTableAuthorizedAsync(
        this HttpClient client,
        Guid tableId,
        string tablePassword,
        string token)
    {
        var request = new JoinTableAuthorizedRequest
        {
            Password = tablePassword
        };

        return await client.PostAsJsonWithoutDeserializeAsync($"/api/tables/{tableId}/join", request, token);
    }

    /// <summary>
    ///     Joins a table as an authenticated user with wrong password for validation testing.
    ///     For snapshot testing - doesn't deserialize the response.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table to join.</param>
    /// <param name="token">The authentication token of the user.</param>
    /// <returns>HttpResponseMessage from the authorized join endpoint.</returns>
    public static async Task<HttpResponseMessage> JoinTableAuthorizedWithWrongPasswordAsync(
        this HttpClient client,
        Guid tableId,
        string token)
    {
        var request = new JoinTableAuthorizedRequest
        {
            Password = TestConstants.WrongPassword
        };

        return await client.PostAsJsonWithoutDeserializeAsync($"/api/tables/{tableId}/join", request, token);
    }

    /// <summary>
    ///     Joins a table as an authenticated user without token (should return unauthorized).
    ///     For snapshot testing - doesn't deserialize the response.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="tableId">The ID of the table to join.</param>
    /// <param name="tablePassword">The password of the table.</param>
    /// <returns>HttpResponseMessage from the authorized join endpoint.</returns>
    public static async Task<HttpResponseMessage> JoinTableAuthorizedWithoutTokenAsync(
        this HttpClient client,
        Guid tableId,
        string tablePassword)
    {
        var request = new JoinTableAuthorizedRequest
        {
            Password = tablePassword
        };

        return await client.PostAsJsonWithoutDeserializeAsync($"/api/tables/{tableId}/join", request);
    }}