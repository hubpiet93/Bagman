using System.Net.Http.Headers;
using System.Text;
using Bagman.Contracts.Models.Auth;
using Bagman.Contracts.Models.Tables;
using Bagman.IntegrationTests.TestFixtures;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace Bagman.IntegrationTests.Controllers;

[CollectionDefinition("Matches Tests")]
public class MatchesTestsCollection : ICollectionFixture<PostgresFixture>
{
}

/// <summary>
///     Integration tests for MatchesController actions using TestContainers for PostgreSQL.
///     Tests Get Match Details.
///     Note: Match creation/update/deletion is now SuperAdmin-only (see AdminMatchesController tests).
/// </summary>
[Collection("Matches Tests")]
public class MatchesControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    public MatchesControllerTests(PostgresFixture postgresFixture, ITestOutputHelper testOutputHelper) : base(postgresFixture, testOutputHelper)
    {
    }

    public async Task InitializeAsync()
    {
        await Init();
    }

    public new async Task DisposeAsync()
    {
        await Dispose();
    }

    private async Task<(Guid TableId, string UserToken)> CreateTableAndUser(string userLogin, string userPassword, string tableName)
    {
        // Ensure login is unique per test run to avoid collisions
        var userLoginUnique = $"{userLogin}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";

        // Create table (endpoint auto-registers creator)
        var createTableRequest = new CreateTableRequest
        {
            UserLogin = userLoginUnique,
            UserPassword = userPassword,
            TableName = tableName,
            TablePassword = "TablePass@123",
            MaxPlayers = 10,
            Stake = 100m,
            EventTypeId = DefaultEventTypeId
        };

        var createTableContent = new StringContent(
            JsonConvert.SerializeObject(createTableRequest),
            Encoding.UTF8,
            "application/json");

        var createResponse = await HttpClient!.PostAsync("/api/tables", createTableContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();

        if (!createResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to create table: {createResponse.StatusCode} - {createBody}");

        var tableResponse = JsonConvert.DeserializeObject<TableResponse>(createBody);

        // Login user to obtain token
        var loginRequest = new LoginRequest
        {
            Login = userLoginUnique,
            Password = userPassword
        };

        var loginContent = new StringContent(
            JsonConvert.SerializeObject(loginRequest),
            Encoding.UTF8,
            "application/json");

        var loginResponse = await HttpClient.PostAsync("/api/auth/login", loginContent);
        var loginBody = await loginResponse.Content.ReadAsStringAsync();
        if (!loginResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to login user after table creation: {loginResponse.StatusCode} - {loginBody}");

        var authResponse = JsonConvert.DeserializeObject<AuthResponse>(loginBody);
        var userToken = authResponse!.AccessToken;

        return (tableResponse!.Id, userToken);
    }

    private async Task<Guid> CreateMatchAsSuperAdmin(string country1, string country2, DateTime matchDateTime)
    {
        var superAdminToken = await GetSuperAdminToken();
        
        var createMatchRequest = new CreateMatchRequest
        {
            Country1 = country1,
            Country2 = country2,
            MatchDateTime = matchDateTime
        };

        var matchContent = new StringContent(
            JsonConvert.SerializeObject(createMatchRequest),
            Encoding.UTF8,
            "application/json");

        var createMatchHttpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/event-types/{DefaultEventTypeId}/matches")
        {
            Content = matchContent
        };
        createMatchHttpRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", superAdminToken);

        var createMatchResponse = await HttpClient.SendAsync(createMatchHttpRequest);
        var createMatchBody = await createMatchResponse.Content.ReadAsStringAsync();
        
        if (!createMatchResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to create match: {createMatchResponse.StatusCode} - {createMatchBody}");
            
        var matchResponse = JsonConvert.DeserializeObject<MatchResponse>(createMatchBody);

        return matchResponse!.Id;
    }

    [Fact]
    public async Task GetMatch_WithValidId_ReturnsOkWithMatchResponse()
    {
        // Arrange - Create table and match via SuperAdmin
        var (tableId, userToken) = await CreateTableAndUser("match_get", "User@12345", "Get Match Table");
        var matchId = await CreateMatchAsSuperAdmin("Spain", "Portugal", DateTime.UtcNow.AddDays(5));

        // Act - Get match
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/tables/{tableId}/matches/{matchId}");
        getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        await HttpClient!.SendAsync(getRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetMatch_StartedFalse_ForFutureDateTime()
    {
        // Arrange - Create table and match with future date
        var (tableId, userToken) = await CreateTableAndUser("match_started_future", "User@12345", "Started Future Table");
        var matchId = await CreateMatchAsSuperAdmin("Poland", "Germany", DateTime.UtcNow.AddHours(2));

        // Act - Get match
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/tables/{tableId}/matches/{matchId}");
        getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        await HttpClient!.SendAsync(getRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetMatch_StartedTrue_ForPastDateTime()
    {
        // Arrange - Create table and match 
        // Note: For testing "Started = true", we need a match with datetime in the past relative to now
        // But CreateMatch validation requires future dates, so we create it with future date first,
        // then test using a snapshot at a later time. For this test we'll use a date that was future when created.
        var (tableId, userToken) = await CreateTableAndUser("match_started_past", "User@12345", "Started Past Table");
        var matchId = await CreateMatchAsSuperAdmin("France", "Italy", DateTime.UtcNow.AddSeconds(5));

        // Wait a bit so the match datetime becomes "in the past" relative to current time
        await Task.Delay(6000);

        // Act - Get match
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/tables/{tableId}/matches/{matchId}");
        getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        await HttpClient!.SendAsync(getRequest);

        // Assert
        await VerifyHttpRecording();
    }
}
