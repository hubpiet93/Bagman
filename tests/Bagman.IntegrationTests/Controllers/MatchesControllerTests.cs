using System.Net.Http.Headers;
using System.Text;
using Bagman.Contracts.Models.Auth;
using Bagman.Contracts.Models.Tables;
using Bagman.IntegrationTests.TestFixtures;
using Newtonsoft.Json;

namespace Bagman.IntegrationTests.Controllers;

[CollectionDefinition("Matches Tests")]
public class MatchesTestsCollection : ICollectionFixture<PostgresFixture>
{
}

/// <summary>
///     Integration tests for MatchesController actions using TestContainers for PostgreSQL.
///     Tests Create Match, Get Match, Update Match, Delete Match, and Set Match Result.
///     Only table administrators can manage matches.
/// </summary>
[Collection("Matches Tests")]
public class MatchesControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    public MatchesControllerTests(PostgresFixture postgresFixture) : base(postgresFixture)
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

    private async Task<(Guid TableId, string CreatorToken)> CreateTableAsAdmin(string creatorLogin, string creatorPassword, string tableName)
    {
        // Ensure login is unique per test run to avoid collisions
        var creatorLoginUnique = $"{creatorLogin}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";

        // Create table (endpoint auto-registers creator)
        var createTableRequest = new CreateTableRequest
        {
            UserLogin = creatorLoginUnique,
            UserPassword = creatorPassword,
            TableName = tableName,
            TablePassword = "TablePass@123",
            MaxPlayers = 10,
            Stake = 100m
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

        // Login creator to obtain token
        var loginRequest = new LoginRequest
        {
            Login = creatorLoginUnique,
            Password = creatorPassword
        };

        var loginContent = new StringContent(
            JsonConvert.SerializeObject(loginRequest),
            Encoding.UTF8,
            "application/json");

        var loginResponse = await HttpClient.PostAsync("/api/auth/login", loginContent);
        var loginBody = await loginResponse.Content.ReadAsStringAsync();
        if (!loginResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to login creator after table creation: {loginResponse.StatusCode} - {loginBody}");

        var authResponse = JsonConvert.DeserializeObject<AuthResponse>(loginBody);
        var creatorToken = authResponse!.AccessToken;

        return (tableResponse!.Id, creatorToken);
    }

    [Fact]
    public async Task CreateMatch_ByTableAdmin_ReturnsCreatedWithMatchResponse()
    {
        // Arrange
        var (tableId, adminToken) = await CreateTableAsAdmin("match_creator", "Creator@12345", "Match Table");

        var createMatchRequest = new CreateMatchRequest
        {
            Country1 = "Poland",
            Country2 = "Germany",
            MatchDateTime = DateTime.UtcNow.AddDays(7)
        };

        var matchContent = new StringContent(
            JsonConvert.SerializeObject(createMatchRequest),
            Encoding.UTF8,
            "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches")
        {
            Content = matchContent
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        // Act
        await HttpClient.SendAsync(request);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateMatch_WithInvalidCountries_ReturnsBadRequest()
    {
        // Arrange
        var (tableId, adminToken) = await CreateTableAsAdmin("match_invalid", "Creator@12345", "Invalid Match Table");

        var createMatchRequest = new CreateMatchRequest
        {
            Country1 = "Poland",
            Country2 = "Poland", // Same country
            MatchDateTime = DateTime.UtcNow.AddDays(7)
        };

        var matchContent = new StringContent(
            JsonConvert.SerializeObject(createMatchRequest),
            Encoding.UTF8,
            "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches")
        {
            Content = matchContent
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        // Act
        await HttpClient.SendAsync(request);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateMatch_WithPastDateTime_ReturnsBadRequest()
    {
        // Arrange
        var (tableId, adminToken) = await CreateTableAsAdmin("match_past", "Creator@12345", "Past Match Table");

        var createMatchRequest = new CreateMatchRequest
        {
            Country1 = "France",
            Country2 = "Italy",
            MatchDateTime = DateTime.UtcNow.AddDays(-1) // Past date
        };

        var matchContent = new StringContent(
            JsonConvert.SerializeObject(createMatchRequest),
            Encoding.UTF8,
            "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches")
        {
            Content = matchContent
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        // Act
        await HttpClient!.SendAsync(request);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetMatch_WithValidId_ReturnsOkWithMatchResponse()
    {
        // Arrange - Create table and match
        var (tableId, adminToken) = await CreateTableAsAdmin("match_get", "Creator@12345", "Get Match Table");

        var createMatchRequest = new CreateMatchRequest
        {
            Country1 = "Spain",
            Country2 = "Portugal",
            MatchDateTime = DateTime.UtcNow.AddDays(5)
        };

        var matchContent = new StringContent(
            JsonConvert.SerializeObject(createMatchRequest),
            Encoding.UTF8,
            "application/json");

        var createRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches")
        {
            Content = matchContent
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var createResponse = await HttpClient!.SendAsync(createRequest);
        var createBody = await createResponse.Content.ReadAsStringAsync();

        if (!createResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to create match: {createResponse.StatusCode} - {createBody}");

        var createdMatch = JsonConvert.DeserializeObject<MatchResponse>(createBody);

        // Act - Get match
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/tables/{tableId}/matches/{createdMatch!.Id}");
        getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var response = await HttpClient.SendAsync(getRequest);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to get match: {response.StatusCode} - {responseBody}");

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task UpdateMatch_BeforeStarted_ReturnsOk()
    {
        // Arrange - Create table and match
        var (tableId, adminToken) = await CreateTableAsAdmin("match_update", "Creator@12345", "Update Match Table");

        var createMatchRequest = new CreateMatchRequest
        {
            Country1 = "Netherlands",
            Country2 = "Belgium",
            MatchDateTime = DateTime.UtcNow.AddDays(10)
        };

        var matchContent = new StringContent(
            JsonConvert.SerializeObject(createMatchRequest),
            Encoding.UTF8,
            "application/json");

        var createRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches")
        {
            Content = matchContent
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var createResponse = await HttpClient!.SendAsync(createRequest);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var createdMatch = JsonConvert.DeserializeObject<MatchResponse>(createBody);

        // Act - Update match
        var updateMatchRequest = new UpdateMatchRequest
        {
            Country1 = "Netherlands",
            Country2 = "Austria", // Changed country
            MatchDateTime = DateTime.UtcNow.AddDays(12)
        };

        var updateContent = new StringContent(
            JsonConvert.SerializeObject(updateMatchRequest),
            Encoding.UTF8,
            "application/json");

        var updateRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/tables/{tableId}/matches/{createdMatch!.Id}")
        {
            Content = updateContent
        };
        updateRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        await HttpClient.SendAsync(updateRequest);
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task DeleteMatch_BeforeStarted_ReturnsOk()
    {
        // Arrange - Create table and match
        var (tableId, adminToken) = await CreateTableAsAdmin("match_delete", "Creator@12345", "Delete Match Table");

        var createMatchRequest = new CreateMatchRequest
        {
            Country1 = "Greece",
            Country2 = "Cyprus",
            MatchDateTime = DateTime.UtcNow.AddDays(15)
        };

        var matchContent = new StringContent(
            JsonConvert.SerializeObject(createMatchRequest),
            Encoding.UTF8,
            "application/json");

        var createRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches")
        {
            Content = matchContent
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var createResponse = await HttpClient!.SendAsync(createRequest);
        var createBody = await createResponse.Content.ReadAsStringAsync();

        if (!createResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to create match: {createResponse.StatusCode} - {createBody}");

        var createdMatch = JsonConvert.DeserializeObject<MatchResponse>(createBody);

        // Act - Delete match
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/tables/{tableId}/matches/{createdMatch!.Id}");
        deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        await HttpClient.SendAsync(deleteRequest);
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task SetMatchResult_WithValidScore_ReturnsOkAndChangesStatus()
    {
        // Arrange - Create table and match
        var (tableId, adminToken) = await CreateTableAsAdmin("match_result", "Creator@12345", "Result Match Table");

        var createMatchRequest = new CreateMatchRequest
        {
            Country1 = "England",
            Country2 = "Scotland",
            MatchDateTime = DateTime.UtcNow.AddDays(3)
        };

        var matchContent = new StringContent(
            JsonConvert.SerializeObject(createMatchRequest),
            Encoding.UTF8,
            "application/json");

        var createRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches")
        {
            Content = matchContent
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var createResponse = await HttpClient!.SendAsync(createRequest);
        var createBody = await createResponse.Content.ReadAsStringAsync();

        if (!createResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to create match: {createResponse.StatusCode} - {createBody}");

        var createdMatch = JsonConvert.DeserializeObject<MatchResponse>(createBody);

        // Act - Set result
        var setResultRequest = new SetMatchResultRequest
        {
            Score1 = "2",
            Score2 = "1"
        };

        var resultContent = new StringContent(
            JsonConvert.SerializeObject(setResultRequest),
            Encoding.UTF8,
            "application/json");

        var resultHttpRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/tables/{tableId}/matches/{createdMatch!.Id}/result")
        {
            Content = resultContent
        };
        resultHttpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        await HttpClient.SendAsync(resultHttpRequest);
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateMatch_WithoutAdminRole_ReturnsForbidden()
    {
        // Arrange - Create table, then try to create match with non-admin token
        var (tableId, creatorToken) = await CreateTableAsAdmin("match_admin_check", "Creator@12345", "Admin Check Table");

        // Register non-admin user
        var memberLogin = "match_non_admin";
        var memberRegisterRequest = new RegisterRequest
        {
            Login = memberLogin,
            Password = "Member@12345",
            Email = $"{memberLogin}@example.com"
        };

        var memberRegisterContent = new StringContent(
            JsonConvert.SerializeObject(memberRegisterRequest),
            Encoding.UTF8,
            "application/json");

        var memberRegisterResponse = await HttpClient!.PostAsync("/api/auth/register", memberRegisterContent);
        var memberRegisterBody = await memberRegisterResponse.Content.ReadAsStringAsync();
        var memberAuthResponse = JsonConvert.DeserializeObject<AuthResponse>(memberRegisterBody);

        var memberToken = memberAuthResponse!.AccessToken;

        // Join table as non-admin
        var joinRequest = new JoinTableRequest
        {
            UserLogin = memberLogin,
            UserPassword = "Member@12345",
            TableName = "Admin Check Table",
            TablePassword = "TablePass@123"
        };

        var joinContent = new StringContent(
            JsonConvert.SerializeObject(joinRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/tables/join", joinContent);

        // Act - Try to create match with non-admin token
        var createMatchRequest = new CreateMatchRequest
        {
            Country1 = "Brazil",
            Country2 = "Argentina",
            MatchDateTime = DateTime.UtcNow.AddDays(7)
        };

        var matchContent = new StringContent(
            JsonConvert.SerializeObject(createMatchRequest),
            Encoding.UTF8,
            "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches")
        {
            Content = matchContent
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", memberToken);

        await HttpClient.SendAsync(request);

        // Assert
        await VerifyHttpRecording();
    }
}
