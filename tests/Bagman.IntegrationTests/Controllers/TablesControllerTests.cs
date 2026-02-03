using System.Net.Http.Headers;
using System.Text;
using Bagman.Contracts.Models.Auth;
using Bagman.Contracts.Models.Tables;
using Bagman.IntegrationTests.TestFixtures;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace Bagman.IntegrationTests.Controllers;

[CollectionDefinition("Tables Tests")]
public class TablesTestsCollection : ICollectionFixture<PostgresFixture>
{
}

/// <summary>
///     Integration tests for TablesController actions using TestContainers for PostgreSQL.
///     Tests Create Table, Join Table, Get Tables, Get Table Details, Leave Table, and Admin management.
///     Uses collection fixture to share one PostgreSQL container across all tests in the class.
/// </summary>
[Collection("Tables Tests")]
public class TablesControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    public TablesControllerTests(PostgresFixture postgresFixture, ITestOutputHelper testOutputHelper) : base(postgresFixture, testOutputHelper)
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

    private static string Unique(string prefix)
    {
        return $"{prefix}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
    }

    private async Task<(string Token, Guid UserId, string Login)> RegisterAndGetToken(
        string loginPrefix,
        string password,
        string emailPrefix = "test")
    {
        var login = Unique(loginPrefix);

        var registerRequest = new RegisterRequest
        {
            Login = login,
            Password = password,
            Email = $"{emailPrefix}+{login}@example.com"
        };

        var registerContent = new StringContent(
            JsonConvert.SerializeObject(registerRequest),
            Encoding.UTF8,
            "application/json");

        var registerResponse = await HttpClient!.PostAsync("/api/auth/register", registerContent);
        var registerBody = await registerResponse.Content.ReadAsStringAsync();
        var authResponse = JsonConvert.DeserializeObject<AuthResponse>(registerBody);

        return (authResponse!.AccessToken, authResponse.User.Id, login);
    }

    [Fact]
    public async Task CreateTable_WithValidRequest_ReturnsOkWithTableResponse()
    {
        // Arrange
        var request = new CreateTableRequest
        {
            UserLogin = Unique("creator_user"),
            UserPassword = "Creator@12345",
            TableName = $"Test Betting Table {Guid.NewGuid()}",
            TablePassword = "TablePass@123",
            MaxPlayers = 10,
            Stake = 50m,
            EventTypeId = DefaultEventTypeId
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json");

        // Act
        await HttpClient!.PostAsync("/api/tables", content);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateTable_WithInvalidTableName_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateTableRequest
        {
            UserLogin = Unique("user_invalid_table"),
            UserPassword = "Pass@12345",
            TableName = "", // Empty table name
            TablePassword = "TablePass@123",
            MaxPlayers = 10,
            Stake = 50m,
            EventTypeId = DefaultEventTypeId
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json");

        // Act
        await HttpClient!.PostAsync("/api/tables", content);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateTable_WithNegativeStake_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateTableRequest
        {
            UserLogin = Unique("user_negative_stake"),
            UserPassword = "Pass@12345",
            TableName = $"Test Table {Guid.NewGuid()}",
            TablePassword = "TablePass@123",
            MaxPlayers = 10,
            Stake = -50m, // Negative stake
            EventTypeId = DefaultEventTypeId
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json");

        // Act
        await HttpClient!.PostAsync("/api/tables", content);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTable_WithValidRequest_ReturnsOkWithTableResponse()
    {
        // Arrange - Create table first
        var tableName = $"Joinable Table {Guid.NewGuid()}";

        var createRequest = new CreateTableRequest
        {
            UserLogin = Unique("table_creator"),
            UserPassword = "Creator@12345",
            TableName = tableName,
            TablePassword = "JoinPass@123",
            MaxPlayers = 5,
            Stake = 100m,
            EventTypeId = DefaultEventTypeId
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(createRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient!.PostAsync("/api/tables", createContent);

        // Arrange - Join table
        var joinRequest = new JoinTableRequest
        {
            UserLogin = Unique("joining_user"),
            UserPassword = "Joiner@12345",
            TableName = tableName,
            TablePassword = "JoinPass@123"
        };

        var joinContent = new StringContent(
            JsonConvert.SerializeObject(joinRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        await HttpClient.PostAsync("/api/tables/join", joinContent);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTable_WithWrongPassword_ReturnsBadRequest()
    {
        // Arrange - Create table first
        var tableName = $"Protected Table {Guid.NewGuid()}";

        var createRequest = new CreateTableRequest
        {
            UserLogin = Unique("creator_wrong_pass"),
            UserPassword = "Creator@12345",
            TableName = tableName,
            TablePassword = "CorrectPass@123",
            MaxPlayers = 5,
            Stake = 100m,
            EventTypeId = DefaultEventTypeId
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(createRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient!.PostAsync("/api/tables", createContent);

        // Arrange - Try to join with wrong password
        var joinRequest = new JoinTableRequest
        {
            UserLogin = Unique("joiner_wrong_pass"),
            UserPassword = "Joiner@12345",
            TableName = tableName,
            TablePassword = "WrongPass@123" // Wrong password
        };

        var joinContent = new StringContent(
            JsonConvert.SerializeObject(joinRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        await HttpClient.PostAsync("/api/tables/join", joinContent);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTable_WithFullTable_ReturnsBadRequest()
    {
        // Arrange - Create table with MaxPlayers = 1
        var tableName = $"Full Table {Guid.NewGuid()}";

        var createRequest = new CreateTableRequest
        {
            UserLogin = Unique("creator_full_table"),
            UserPassword = "Creator@12345",
            TableName = tableName,
            TablePassword = "FullPass@123",
            MaxPlayers = 1, // Creator is already a member
            Stake = 50m,
            EventTypeId = DefaultEventTypeId
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(createRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient!.PostAsync("/api/tables", createContent);

        // Arrange - Try to join full table
        var joinRequest = new JoinTableRequest
        {
            UserLogin = Unique("joiner_full"),
            UserPassword = "Joiner@12345",
            TableName = tableName,
            TablePassword = "FullPass@123"
        };

        var joinContent = new StringContent(
            JsonConvert.SerializeObject(joinRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        await HttpClient.PostAsync("/api/tables/join", joinContent);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetUserTables_WithMultipleTables_ReturnsOkWithTableList()
    {
        // Arrange - Register user and create multiple tables
        var (token, _, login) = await RegisterAndGetToken("multi_tables_user", "Pass@12345", "multi");

        var createRequest1 = new CreateTableRequest
        {
            UserLogin = login,
            UserPassword = "Pass@12345",
            TableName = $"Table One {Guid.NewGuid()}",
            TablePassword = "Pass@123",
            MaxPlayers = 5,
            Stake = 50m,
            EventTypeId = DefaultEventTypeId
        };

        var createRequest2 = new CreateTableRequest
        {
            UserLogin = login,
            UserPassword = "Pass@12345",
            TableName = $"Table Two {Guid.NewGuid()}",
            TablePassword = "Pass@123",
            MaxPlayers = 10,
            Stake = 100m,
            EventTypeId = DefaultEventTypeId
        };

        var content1 = new StringContent(
            JsonConvert.SerializeObject(createRequest1),
            Encoding.UTF8,
            "application/json");

        var content2 = new StringContent(
            JsonConvert.SerializeObject(createRequest2),
            Encoding.UTF8,
            "application/json");

        await HttpClient!.PostAsync("/api/tables", content1);
        await HttpClient.PostAsync("/api/tables", content2);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/tables");
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        await HttpClient.SendAsync(request);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDetails_WithValidId_ReturnsOkWithTableAndMembers()
    {
        // Arrange - Create table and join with another user
        var (creatorToken, _, creatorLogin) = await RegisterAndGetToken("details_creator", "Creator@12345", "details");

        var tableName = $"Details Table {Guid.NewGuid()}";

        var createRequest = new CreateTableRequest
        {
            UserLogin = creatorLogin,
            UserPassword = "Creator@12345",
            TableName = tableName,
            TablePassword = "DetailsPass@123",
            MaxPlayers = 5,
            Stake = 50m,
            EventTypeId = DefaultEventTypeId
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(createRequest),
            Encoding.UTF8,
            "application/json");

        var createResponse = await HttpClient!.PostAsync("/api/tables", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var createdTable = JsonConvert.DeserializeObject<TableResponse>(createBody);

        var joinRequest = new JoinTableRequest
        {
            UserLogin = Unique("details_joiner"),
            UserPassword = "Joiner@12345",
            TableName = tableName,
            TablePassword = "DetailsPass@123"
        };

        var joinContent = new StringContent(
            JsonConvert.SerializeObject(joinRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/tables/join", joinContent);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/tables/{createdTable!.Id}");
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", creatorToken);

        await HttpClient.SendAsync(request);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task LeaveTable_AsRegularMember_ReturnsOk()
    {
        // Arrange - Create table and join with another user
        var tableName = $"Leave Table {Guid.NewGuid()}";
        var joinerLogin = Unique("leave_joiner");

        var createRequest = new CreateTableRequest
        {
            UserLogin = Unique("leave_creator"),
            UserPassword = "Creator@12345",
            TableName = tableName,
            TablePassword = "LeavePass@123",
            MaxPlayers = 5,
            Stake = 50m,
            EventTypeId = DefaultEventTypeId
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(createRequest),
            Encoding.UTF8,
            "application/json");

        var createResponse = await HttpClient!.PostAsync("/api/tables", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var createdTable = JsonConvert.DeserializeObject<TableResponse>(createBody);

        var (leaveToken, _, registeredJoinerLogin) = await RegisterAndGetToken(
            joinerLogin, "Joiner@12345", "leave");

        var joinRequest = new JoinTableRequest
        {
            UserLogin = registeredJoinerLogin,
            UserPassword = "Joiner@12345",
            TableName = tableName,
            TablePassword = "LeavePass@123"
        };

        var joinContent = new StringContent(
            JsonConvert.SerializeObject(joinRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/tables/join", joinContent);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/tables/{createdTable!.Id}/members");
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", leaveToken);

        await HttpClient.SendAsync(request);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GrantAdmin_ByTableCreator_ReturnsOk()
    {
        // Arrange - Create table and get creator token
        var (creatorToken, _, creatorLogin) = await RegisterAndGetToken("admin_creator", "Creator@12345", "admin");
        var (_, memberId, memberLogin) = await RegisterAndGetToken("admin_member", "Member@12345", "admin");

        var tableName = $"Admin Table {Guid.NewGuid()}";

        var createRequest = new CreateTableRequest
        {
            UserLogin = creatorLogin,
            UserPassword = "Creator@12345",
            TableName = tableName,
            TablePassword = "AdminPass@123",
            MaxPlayers = 5,
            Stake = 50m,
            EventTypeId = DefaultEventTypeId
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(createRequest),
            Encoding.UTF8,
            "application/json");

        var createResponse = await HttpClient!.PostAsync("/api/tables", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var createdTable = JsonConvert.DeserializeObject<TableResponse>(createBody);

        // Join with member
        var joinRequest = new JoinTableRequest
        {
            UserLogin = memberLogin,
            UserPassword = "Member@12345",
            TableName = tableName,
            TablePassword = "AdminPass@123"
        };

        var joinContent = new StringContent(
            JsonConvert.SerializeObject(joinRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/tables/join", joinContent);

        // Act - Grant admin
        var grantRequest = new GrantAdminRequest
        {
            UserId = memberId
        };

        var grantContent = new StringContent(
            JsonConvert.SerializeObject(grantRequest),
            Encoding.UTF8,
            "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{createdTable!.Id}/admins")
        {
            Content = grantContent
        };
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", creatorToken);

        await HttpClient.SendAsync(request);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task RevokeAdmin_ByTableCreator_ReturnsOk()
    {
        // Arrange - Create table, add member, grant admin, then revoke
        var (creatorToken, _, creatorLogin) = await RegisterAndGetToken("revoke_creator", "Creator@12345", "revoke");
        var (_, memberId, memberLogin) = await RegisterAndGetToken("revoke_member", "Member@12345", "revoke");

        var tableName = $"Revoke Table {Guid.NewGuid()}";

        var createRequest = new CreateTableRequest
        {
            UserLogin = creatorLogin,
            UserPassword = "Creator@12345",
            TableName = tableName,
            TablePassword = "RevokePass@123",
            MaxPlayers = 5,
            Stake = 50m,
            EventTypeId = DefaultEventTypeId
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(createRequest),
            Encoding.UTF8,
            "application/json");

        var createResponse = await HttpClient!.PostAsync("/api/tables", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var createdTable = JsonConvert.DeserializeObject<TableResponse>(createBody);

        // Join with member
        var joinRequest = new JoinTableRequest
        {
            UserLogin = memberLogin,
            UserPassword = "Member@12345",
            TableName = tableName,
            TablePassword = "RevokePass@123"
        };

        var joinContent = new StringContent(
            JsonConvert.SerializeObject(joinRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/tables/join", joinContent);

        // Grant admin first
        var grantRequest = new GrantAdminRequest
        {
            UserId = memberId
        };

        var grantContent = new StringContent(
            JsonConvert.SerializeObject(grantRequest),
            Encoding.UTF8,
            "application/json");

        var grantHttpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{createdTable!.Id}/admins")
        {
            Content = grantContent
        };
        grantHttpRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", creatorToken);

        await HttpClient.SendAsync(grantHttpRequest);

        // Act - Revoke admin
        var revokeRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/tables/{createdTable.Id}/admins/{memberId}");
        revokeRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", creatorToken);

        await HttpClient.SendAsync(revokeRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task AuthorizedCreateTable_WithValidTokenAndRequest_ReturnsCreated()
    {
        // Arrange
        var (token, _, _) = await RegisterAndGetToken("auth_create_user", "Pass@12345", "authcreate");
        var request = new AuthorizedCreateTableRequest
        {
            TableName = $"Authorized Table {Guid.NewGuid()}",
            TablePassword = "AuthTablePass@123",
            MaxPlayers = 10,
            Stake = 25m,
            EventTypeId = DefaultEventTypeId
        };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        var http = new HttpRequestMessage(HttpMethod.Post, "/api/tables/create") { Content = content };
        http.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        await HttpClient!.SendAsync(http);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task AuthorizedCreateTable_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var request = new AuthorizedCreateTableRequest
        {
            TableName = $"NoToken Table {Guid.NewGuid()}",
            TablePassword = "NoTokenPass@123",
            MaxPlayers = 5,
            Stake = 10m,
            EventTypeId = DefaultEventTypeId
        };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        // Act
        await HttpClient!.PostAsync("/api/tables/create", content);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task AuthorizedCreateTable_WithDuplicateName_ReturnsConflict409()
    {
        // Arrange
        var (token, _, _) = await RegisterAndGetToken("dup_create_user", "Pass@12345", "dupcreate");
        var tableName = $"Duplicate Name Table {Guid.NewGuid()}";
        var first = new AuthorizedCreateTableRequest
        {
            TableName = tableName,
            TablePassword = "DupPass@123",
            MaxPlayers = 5,
            Stake = 10m,
            EventTypeId = DefaultEventTypeId
        };
        var firstContent = new StringContent(JsonConvert.SerializeObject(first), Encoding.UTF8, "application/json");
        var firstHttp = new HttpRequestMessage(HttpMethod.Post, "/api/tables/create") { Content = firstContent };
        firstHttp.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await HttpClient!.SendAsync(firstHttp);

        var second = new AuthorizedCreateTableRequest
        {
            TableName = tableName,
            TablePassword = "DupPass@123",
            MaxPlayers = 5,
            Stake = 10m,
            EventTypeId = DefaultEventTypeId
        };
        var secondContent = new StringContent(JsonConvert.SerializeObject(second), Encoding.UTF8, "application/json");
        var secondHttp = new HttpRequestMessage(HttpMethod.Post, "/api/tables/create") { Content = secondContent };
        secondHttp.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        await HttpClient.SendAsync(secondHttp);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task AuthorizedCreateTable_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var (token, _, _) = await RegisterAndGetToken("invalid_create_user", "Pass@12345", "invalidcreate");
        var request = new AuthorizedCreateTableRequest
        {
            TableName = "", // invalid
            TablePassword = "", // invalid
            MaxPlayers = 0, // invalid
            Stake = -1m, // invalid
            EventTypeId = DefaultEventTypeId
        };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        var http = new HttpRequestMessage(HttpMethod.Post, "/api/tables/create") { Content = content };
        http.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        await HttpClient!.SendAsync(http);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_WithValidMember_ReturnsOkWithDashboardData()
    {
        // Arrange - Create table with creator and member
        var (creatorToken, _, creatorLogin) = await RegisterAndGetToken("dash_creator", "Creator@12345", "dashboard");
        var (memberToken, _, memberLogin) = await RegisterAndGetToken("dash_member", "Member@12345", "dashboard");

        var tableName = $"Dashboard Table {Guid.NewGuid()}";

        // Create table
        var createRequest = new CreateTableRequest
        {
            UserLogin = creatorLogin,
            UserPassword = "Creator@12345",
            TableName = tableName,
            TablePassword = "DashPass@123",
            MaxPlayers = 5,
            Stake = 50m,
            EventTypeId = DefaultEventTypeId
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(createRequest),
            Encoding.UTF8,
            "application/json");

        var createResponse = await HttpClient!.PostAsync("/api/tables", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var createdTable = JsonConvert.DeserializeObject<TableResponse>(createBody);

        // Join table with member
        var joinRequest = new JoinTableRequest
        {
            UserLogin = memberLogin,
            UserPassword = "Member@12345",
            TableName = tableName,
            TablePassword = "DashPass@123"
        };

        var joinContent = new StringContent(
            JsonConvert.SerializeObject(joinRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/tables/join", joinContent);

        // Act - Get dashboard as member
        var dashboardRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/tables/{createdTable!.Id}/dashboard");
        dashboardRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", memberToken);

        await HttpClient.SendAsync(dashboardRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var tableId = Guid.NewGuid();

        // Act - Try to get dashboard without token
        await HttpClient!.GetAsync($"/api/tables/{tableId}/dashboard");

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_AsNonMember_ReturnsForbidden()
    {
        // Arrange - Create table with creator
        var (creatorToken, _, creatorLogin) = await RegisterAndGetToken("dash_creator_forbidden", "Creator@12345", "dashforbid");
        var (nonMemberToken, _, _) = await RegisterAndGetToken("dash_non_member", "NonMember@12345", "dashforbid");

        var tableName = $"Forbidden Table {Guid.NewGuid()}";

        var createRequest = new CreateTableRequest
        {
            UserLogin = creatorLogin,
            UserPassword = "Creator@12345",
            TableName = tableName,
            TablePassword = "ForbidPass@123",
            MaxPlayers = 5,
            Stake = 50m,
            EventTypeId = DefaultEventTypeId
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(createRequest),
            Encoding.UTF8,
            "application/json");

        var createResponse = await HttpClient!.PostAsync("/api/tables", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var createdTable = JsonConvert.DeserializeObject<TableResponse>(createBody);

        // Act - Try to get dashboard as non-member
        var dashboardRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/tables/{createdTable!.Id}/dashboard");
        dashboardRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", nonMemberToken);

        await HttpClient.SendAsync(dashboardRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_WithNonExistentTable_ReturnsNotFound()
    {
        // Arrange
        var (token, _, _) = await RegisterAndGetToken("dash_nonexistent", "Pass@12345", "dashne");
        var nonExistentTableId = Guid.NewGuid();

        // Act
        var dashboardRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/tables/{nonExistentTableId}/dashboard");
        dashboardRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await HttpClient!.SendAsync(dashboardRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_IncludesTableInfo_Members_AndEmptyData()
    {
        // Arrange - Create a table as the only member
        var (token, _, creatorLogin) = await RegisterAndGetToken("dash_solo", "Pass@12345", "dashsolo");

        var tableName = $"Solo Dashboard Table {Guid.NewGuid()}";

        var createRequest = new CreateTableRequest
        {
            UserLogin = creatorLogin,
            UserPassword = "Pass@12345",
            TableName = tableName,
            TablePassword = "SoloPass@123",
            MaxPlayers = 5,
            Stake = 100m,
            EventTypeId = DefaultEventTypeId
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(createRequest),
            Encoding.UTF8,
            "application/json");

        var createResponse = await HttpClient!.PostAsync("/api/tables", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var createdTable = JsonConvert.DeserializeObject<TableResponse>(createBody);

        // Act - Get dashboard should return table info with empty matches/bets/pools/stats
        var dashboardRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/tables/{createdTable!.Id}/dashboard");
        dashboardRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await HttpClient.SendAsync(dashboardRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_WithFullData_ReturnsMatchesBetsAndStats()
    {
        // Arrange - Create table with multiple members
        var (creatorToken, creatorId, creatorLogin) = await RegisterAndGetToken("fulldata_creator", "Creator@12345", "fulldata");
        var (member1Token, member1Id, member1Login) = await RegisterAndGetToken("fulldata_member1", "Member1@12345", "fulldata");
        var (member2Token, member2Id, member2Login) = await RegisterAndGetToken("fulldata_member2", "Member2@12345", "fulldata");

        var tableName = $"Full Data Table {Guid.NewGuid()}";

        // Create table
        var createRequest = new CreateTableRequest
        {
            UserLogin = creatorLogin,
            UserPassword = "Creator@12345",
            TableName = tableName,
            TablePassword = "FullDataPass@123",
            MaxPlayers = 10,
            Stake = 50m,
            EventTypeId = DefaultEventTypeId
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(createRequest),
            Encoding.UTF8,
            "application/json");

        var createResponse = await HttpClient!.PostAsync("/api/tables", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var createdTable = JsonConvert.DeserializeObject<TableResponse>(createBody);

        // Join table with members
        var joinRequest1 = new JoinTableRequest
        {
            UserLogin = member1Login,
            UserPassword = "Member1@12345",
            TableName = tableName,
            TablePassword = "FullDataPass@123"
        };

        var joinContent1 = new StringContent(
            JsonConvert.SerializeObject(joinRequest1),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/tables/join", joinContent1);

        var joinRequest2 = new JoinTableRequest
        {
            UserLogin = member2Login,
            UserPassword = "Member2@12345",
            TableName = tableName,
            TablePassword = "FullDataPass@123"
        };

        var joinContent2 = new StringContent(
            JsonConvert.SerializeObject(joinRequest2),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/tables/join", joinContent2);

        // Get SuperAdmin token and create matches
        var superAdminToken = await GetSuperAdminToken();
        
        // Create 2 matches
        var match1DateTime = DateTime.UtcNow.AddDays(1);
        var match2DateTime = DateTime.UtcNow.AddDays(2);

        var createMatch1Request = new CreateMatchRequest
        {
            Country1 = "Poland",
            Country2 = "Spain",
            MatchDateTime = match1DateTime
        };

        var matchContent1 = new StringContent(
            JsonConvert.SerializeObject(createMatch1Request),
            Encoding.UTF8,
            "application/json");

        var createMatch1Http = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/event-types/{DefaultEventTypeId}/matches")
        {
            Content = matchContent1
        };
        createMatch1Http.Headers.Authorization = new AuthenticationHeaderValue("Bearer", superAdminToken);

        var createMatch1Response = await HttpClient.SendAsync(createMatch1Http);
        var createMatch1Body = await createMatch1Response.Content.ReadAsStringAsync();
        var match1 = JsonConvert.DeserializeObject<MatchResponse>(createMatch1Body);

        var createMatch2Request = new CreateMatchRequest
        {
            Country1 = "Germany",
            Country2 = "France",
            MatchDateTime = match2DateTime
        };

        var matchContent2 = new StringContent(
            JsonConvert.SerializeObject(createMatch2Request),
            Encoding.UTF8,
            "application/json");

        var createMatch2Http = new HttpRequestMessage(HttpMethod.Post, $"/api/admin/event-types/{DefaultEventTypeId}/matches")
        {
            Content = matchContent2
        };
        createMatch2Http.Headers.Authorization = new AuthenticationHeaderValue("Bearer", superAdminToken);

        var createMatch2Response = await HttpClient.SendAsync(createMatch2Http);
        var createMatch2Body = await createMatch2Response.Content.ReadAsStringAsync();
        var match2 = JsonConvert.DeserializeObject<MatchResponse>(createMatch2Body);

        // Place bets from different users
        var bet1Request = new PlaceBetRequest { Prediction = "1:0" };
        var bet1Content = new StringContent(
            JsonConvert.SerializeObject(bet1Request),
            Encoding.UTF8,
            "application/json");

        var placeBet1Http = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{createdTable!.Id}/matches/{match1!.Id}/bets")
        {
            Content = bet1Content
        };
        placeBet1Http.Headers.Authorization = new AuthenticationHeaderValue("Bearer", creatorToken);
        await HttpClient.SendAsync(placeBet1Http);

        // Member 1 bets on match 1
        var bet2Request = new PlaceBetRequest { Prediction = "2:1" };
        var bet2Content = new StringContent(
            JsonConvert.SerializeObject(bet2Request),
            Encoding.UTF8,
            "application/json");

        var placeBet2Http = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{createdTable.Id}/matches/{match1.Id}/bets")
        {
            Content = bet2Content
        };
        placeBet2Http.Headers.Authorization = new AuthenticationHeaderValue("Bearer", member1Token);
        await HttpClient.SendAsync(placeBet2Http);

        // Member 2 bets on match 2
        var bet3Request = new PlaceBetRequest { Prediction = "X" };
        var bet3Content = new StringContent(
            JsonConvert.SerializeObject(bet3Request),
            Encoding.UTF8,
            "application/json");

        var placeBet3Http = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{createdTable.Id}/matches/{match2!.Id}/bets")
        {
            Content = bet3Content
        };
        placeBet3Http.Headers.Authorization = new AuthenticationHeaderValue("Bearer", member2Token);
        await HttpClient.SendAsync(placeBet3Http);

        // Act - Get dashboard as creator (should see all members and all bets)
        var dashboardRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/tables/{createdTable.Id}/dashboard");
        dashboardRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", creatorToken);

        await HttpClient.SendAsync(dashboardRequest);

        // Assert
        await VerifyHttpRecording();
    }
}
