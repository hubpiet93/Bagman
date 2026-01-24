using System.Net.Http.Headers;
using System.Text;
using Bagman.Contracts.Models.Auth;
using Bagman.Contracts.Models.Tables;
using Bagman.IntegrationTests.TestFixtures;
using Newtonsoft.Json;

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
    public TablesControllerTests(PostgresFixture postgresFixture) : base(postgresFixture)
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
            Stake = 50m
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
            Stake = 50m
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
            Stake = -50m // Negative stake
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
            Stake = 100m
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
            Stake = 100m
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
            Stake = 50m
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
            Stake = 50m
        };

        var createRequest2 = new CreateTableRequest
        {
            UserLogin = login,
            UserPassword = "Pass@12345",
            TableName = $"Table Two {Guid.NewGuid()}",
            TablePassword = "Pass@123",
            MaxPlayers = 10,
            Stake = 100m
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
            Stake = 50m
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
            Stake = 50m
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
            Stake = 50m
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
            Stake = 50m
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
}
