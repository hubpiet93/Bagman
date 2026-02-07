using Bagman.Contracts.Models.Tables;
using Bagman.IntegrationTests.Controllers.Endpoints;
using Bagman.IntegrationTests.Controllers.Endpoints.Bets;
using Bagman.IntegrationTests.Controllers.Endpoints.EventTypes;
using Bagman.IntegrationTests.Controllers.Endpoints.Matches;
using Bagman.IntegrationTests.Controllers.Endpoints.Tables;
using Bagman.IntegrationTests.Helpers;
using Bagman.IntegrationTests.TestFixtures;
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

    [Fact]
    public async Task CreateTable_WithValidRequest_ReturnsCreatedWithTableResponse()
    {
        // Act
        var request = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId);
        await HttpClient.CreateTableAsync<HttpResponseMessage>(request);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateTable_WithInvalidTableName_ReturnsBadRequest()
    {
        // Act - Create request with empty table name (invalid)
        var request = new CreateTableRequest
        {
            UserLogin = AuthEndpointsHelpers.Unique("invalid_name_user"),
            UserPassword = TestConstants.CreatorPassword,
            TableName = "",  // invalid - empty name
            TablePassword = TestConstants.DefaultTablePassword,
            MaxPlayers = TestConstants.DefaultMaxPlayers,
            Stake = TestConstants.DefaultStake,
            EventTypeId = DefaultEventTypeId
        };
        await HttpClient.CreateTableAsync<HttpResponseMessage>(request);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateTable_WithNegativeStake_ReturnsBadRequest()
    {
        // Act - Create request with negative stake (invalid)
        var request = new CreateTableRequest
        {
            UserLogin = AuthEndpointsHelpers.Unique("negative_stake_user"),
            UserPassword = TestConstants.CreatorPassword,
            TableName = $"Negative Stake Table {Guid.NewGuid()}",
            TablePassword = TestConstants.DefaultTablePassword,
            MaxPlayers = TestConstants.DefaultMaxPlayers,
            Stake = -50m,  // invalid - negative stake
            EventTypeId = DefaultEventTypeId
        };
        await HttpClient.CreateTableAsync<HttpResponseMessage>(request);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTable_WithValidRequest_ReturnsOkWithTableResponse()
    {
        // Arrange
        var tableName = $"Joinable Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, tableName: tableName);
        await HttpClient.CreateTableAsync<HttpResponseMessage>(createRequest);

        // Act
        var joinRequest = TableJoinHelpers.CreateJoinTableRequest(tableName, TestConstants.DefaultTablePassword);
        await HttpClient.JoinTableAsync<HttpResponseMessage>(joinRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTable_WithWrongPassword_ReturnsBadRequest()
    {
        // Arrange
        var tableName = $"Protected Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, tableName: tableName, userLogin: AuthEndpointsHelpers.Unique("creator_wrong_pass"));
        await HttpClient.CreateTableAsync<HttpResponseMessage>(createRequest);

        // Act
        var joinRequest = TableJoinHelpers.CreateJoinTableRequest(tableName, TestConstants.WrongPassword);
        await HttpClient.JoinTableAsync<HttpResponseMessage>(joinRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTable_WithFullTable_ReturnsBadRequest()
    {
        // Arrange
        var tableName = $"Full Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, tableName: tableName, maxPlayers: 1);
        await HttpClient.CreateTableAsync<HttpResponseMessage>(createRequest);

        // Act
        var joinRequest = TableJoinHelpers.CreateJoinTableRequest(tableName, TestConstants.DefaultTablePassword);
        await HttpClient.JoinTableAsync<HttpResponseMessage>(joinRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetUserTables_WithMultipleTables_ReturnsOkWithTableList()
    {
        // Arrange - Register user and create multiple tables
        var (token, _, login) = await HttpClient.RegisterAndGetTokenAsync("multi_tables_user", "Pass@12345", "multi");

        var request1 = TableCreationHelpers.CreateDefaultAuthorizedTableRequest(DefaultEventTypeId);
        var request2 = TableCreationHelpers.CreateDefaultAuthorizedTableRequest(DefaultEventTypeId);
        await HttpClient.CreateAuthorizedTableAsync<TableResponse>(request1, token);
        await HttpClient.CreateAuthorizedTableAsync<TableResponse>(request2, token);

        // Act
        await HttpClient.GetUserTablesAsync<HttpResponseMessage>(token);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDetails_WithValidId_ReturnsOkWithTableAndMembers()
    {
        // Arrange - Create table and join with another user
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("details_creator", "Creator@12345", "details");

        var tableName = $"Details Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        var joinRequest = TableJoinHelpers.CreateJoinTableRequest(tableName, "DetailsPass@123");
        await HttpClient.JoinTableAsync<HttpResponseMessage>(joinRequest);

        // Act
        await HttpClient.GetTableDetailsAsync<HttpResponseMessage>(createdTable.Id, creatorToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task LeaveTable_AsRegularMember_ReturnsOk()
    {
        // Arrange - Create table and join with another user
        var tableName = $"Leave Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);
        var (leaveToken, _, _) = await HttpClient.JoinTableAsNewUserAsync(tableName, TestConstants.DefaultTablePassword);

        // Act
        await HttpClient.LeaveTableAsync<HttpResponseMessage>(createdTable.Id, leaveToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GrantAdmin_ByTableCreator_ReturnsOk()
    {
        // Arrange - Create table and get creator token
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("admin_creator", "Creator@12345", "admin");
        var (_, memberId, memberLogin) = await HttpClient.RegisterAndGetTokenAsync("admin_member", "Member@12345", "admin");

        var tableName = $"Admin Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Join with member
        await HttpClient.JoinTableAsExistingUserAsync<HttpResponseMessage>(tableName, "TablePass@123", memberLogin, "Member@12345");

        // Act - Grant admin
        var grantRequest = new GrantAdminRequest { UserId = memberId };
        await HttpClient.GrantAdminAsync<HttpResponseMessage>(createdTable.Id, grantRequest, creatorToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task RevokeAdmin_ByTableCreator_ReturnsOk()
    {
        // Arrange - Create table, add member, grant admin, then revoke
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("revoke_creator", "Creator@12345", "revoke");
        var (_, memberId, memberLogin) = await HttpClient.RegisterAndGetTokenAsync("revoke_member", "Member@12345", "revoke");

        var tableName = $"Revoke Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Join with member
        await HttpClient.JoinTableAsExistingUserAsync<HttpResponseMessage>(tableName, "TablePass@123", memberLogin, "Member@12345");

        // Grant admin first
        var grantRequest = new GrantAdminRequest { UserId = memberId };
        await HttpClient.GrantAdminAsync<HttpResponseMessage>(createdTable.Id, grantRequest, creatorToken);

        // Act - Revoke admin
        await HttpClient.RevokeAdminAsync<HttpResponseMessage>(createdTable.Id, memberId, creatorToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task AuthorizedCreateTable_WithValidTokenAndRequest_ReturnsCreated()
    {
        // Arrange
        var (token, _, _) = await HttpClient.RegisterAndGetTokenAsync("auth_create_user", "Pass@12345", "authcreate");
        var request = TableCreationHelpers.CreateDefaultAuthorizedTableRequest(DefaultEventTypeId);

        // Act
        await HttpClient.CreateAuthorizedTableAsync<HttpResponseMessage>(request, token);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task AuthorizedCreateTable_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var request = TableCreationHelpers.CreateDefaultAuthorizedTableRequest(DefaultEventTypeId);

        // Act
        await HttpClient.CreateAuthorizedTableAsync<HttpResponseMessage>(request, token: null);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task AuthorizedCreateTable_WithDuplicateName_ReturnsConflict409()
    {
        // Arrange
        var (token, _, _) = await HttpClient.RegisterAndGetTokenAsync("dup_create_user", "Pass@12345", "dupcreate");
        var tableName = $"Duplicate Name Table {Guid.NewGuid()}";

        var first = TableCreationHelpers.CreateDefaultAuthorizedTableRequest(DefaultEventTypeId, tableName);
        await HttpClient.CreateAuthorizedTableAsync<TableResponse>(first, token);

        var second = TableCreationHelpers.CreateDefaultAuthorizedTableRequest(DefaultEventTypeId, tableName);

        // Act
        await HttpClient.CreateAuthorizedTableAsync<HttpResponseMessage>(second, token);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task AuthorizedCreateTable_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var (token, _, _) = await HttpClient.RegisterAndGetTokenAsync("invalid_create_user", "Pass@12345", "invalidcreate");

        // Create request with empty table name (invalid)
        var request = new AuthorizedCreateTableRequest
        {
            TableName = "",  // invalid - empty name
            TablePassword = TestConstants.AuthTablePassword,
            MaxPlayers = TestConstants.DefaultMaxPlayers,
            Stake = TestConstants.AuthTableStake,
            EventTypeId = DefaultEventTypeId
        };

        // Act
        await HttpClient.CreateAuthorizedTableAsync<HttpResponseMessage>(request, token);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_WithValidMember_ReturnsOkWithDashboardData()
    {
        // Arrange - Create table with creator and member
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("dash_creator", "Creator@12345", "dashboard");
        var (memberToken, _, memberLogin) = await HttpClient.RegisterAndGetTokenAsync("dash_member", "Member@12345", "dashboard");

        var tableName = $"Dashboard Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Join table with member
        await HttpClient.JoinTableAsExistingUserAsync<HttpResponseMessage>(tableName, "TablePass@123", memberLogin, "Member@12345");

        // Act - Get dashboard as member
        await HttpClient.GetTableDashboardAsync<HttpResponseMessage>(createdTable.Id, memberToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var tableId = Guid.NewGuid();

        // Act - Try to get dashboard without token
        await HttpClient.GetTableDashboardAsync<HttpResponseMessage>(tableId, token: null);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_AsNonMember_ReturnsForbidden()
    {
        // Arrange - Create table with creator
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("dash_creator_forbidden", "Creator@12345", "dashforbid");
        var (nonMemberToken, _, _) = await HttpClient.RegisterAndGetTokenAsync("dash_non_member", "NonMember@12345", "dashforbid");

        var tableName = $"Forbidden Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Act - Try to get dashboard as non-member
        await HttpClient.GetTableDashboardAsync<HttpResponseMessage>(createdTable.Id, nonMemberToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_WithNonExistentTable_ReturnsNotFound()
    {
        // Arrange
        var (token, _, _) = await HttpClient.RegisterAndGetTokenAsync("dash_nonexistent", "Pass@12345", "dashne");
        var nonExistentTableId = Guid.NewGuid();

        // Act
        await HttpClient.GetTableDashboardAsync<HttpResponseMessage>(nonExistentTableId, token);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_IncludesTableInfo_Members_AndEmptyData()
    {
        // Arrange - Create a table as the only member
        var (token, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("dash_solo", "Pass@12345", "dashsolo");

        var tableName = $"Solo Dashboard Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName, userPassword: "Pass@12345");
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Act - Get dashboard should return table info with empty matches/bets/pools/stats
        await HttpClient.GetTableDashboardAsync<HttpResponseMessage>(createdTable.Id, token);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_WithFullData_ReturnsMatchesBetsAndStats()
    {
        // Arrange - Create table with multiple members
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("fulldata_creator", "Creator@12345", "fulldata");
        var (member1Token, _, member1Login) = await HttpClient.RegisterAndGetTokenAsync("fulldata_member1", "Member1@12345", "fulldata");
        var (member2Token, _, member2Login) = await HttpClient.RegisterAndGetTokenAsync("fulldata_member2", "Member2@12345", "fulldata");

        var tableName = $"Full Data Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Join table with members
        await HttpClient.JoinTableAsExistingUserAsync<HttpResponseMessage>(tableName, "TablePass@123", member1Login, "Member1@12345");
        await HttpClient.JoinTableAsExistingUserAsync<HttpResponseMessage>(tableName, "TablePass@123", member2Login, "Member2@12345");

        // Get SuperAdmin token and create matches
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var match1Request = EventTypeMatchCreation.CreateMatchRequest("Poland", "Spain", DateTime.UtcNow.AddDays(1));
        var match2Request = EventTypeMatchCreation.CreateMatchRequest("Germany", "France", DateTime.UtcNow.AddDays(2));
        var match1 = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, match1Request, superAdminToken);
        var match2 = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, match2Request, superAdminToken);

        // Place bets from different users
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match1.Id, new PlaceBetRequest { Prediction = "1:0" }, creatorToken);
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match1.Id, new PlaceBetRequest { Prediction = "2:1" }, member1Token);
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match2.Id, new PlaceBetRequest { Prediction = "X" }, member2Token);

        // Act - Get dashboard as creator (should see all members and all bets)
        await HttpClient.GetTableDashboardAsync<HttpResponseMessage>(createdTable.Id, creatorToken);

        // Assert
        await VerifyHttpRecording();
    }

    #region Authenticated Join Table Tests

    /// <summary>
    ///     Tests for the authorized join table endpoint: POST /api/tables/{tableId}/join
    /// </summary>

    [Fact]
    public async Task JoinTableAuthorized_WithValidTokenAndPassword_ReturnsOkWithJoinTableResponse()
    {
        // Arrange - Create table as anonymous user
        var tableName = $"Authorized Join Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Register and get token for the joining user
        var (joinerToken, joinerUserId, joinerLogin) = await HttpClient.RegisterAndGetTokenAsync("auth_joiner", "Joiner@12345", "authjoin");

        // Act
        var joinRequest = TableJoinHelpers.CreateJoinTableAuthorizedRequest(TestConstants.DefaultTablePassword);
        await HttpClient.JoinTableAuthorizedAsync<HttpResponseMessage>(createdTable.Id, joinRequest, joinerToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTableAuthorized_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange - Create table
        var tableName = $"Unauthorized Join Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Act - Try to join without token
        var joinRequest = TableJoinHelpers.CreateJoinTableAuthorizedRequest(TestConstants.DefaultTablePassword);
        await HttpClient.JoinTableAuthorizedAsync<HttpResponseMessage>(createdTable.Id, joinRequest, token: null);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTableAuthorized_WithWrongPassword_ReturnsBadRequest()
    {
        // Arrange - Create table
        var tableName = $"Wrong Password Join Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Register user with valid token
        var (joinerToken, _, _) = await HttpClient.RegisterAndGetTokenAsync("auth_joiner_wrong_pass", "Joiner@12345", "authwrongpass");

        // Act - Try to join with wrong password
        var joinRequest = TableJoinHelpers.CreateJoinTableAuthorizedRequest(TestConstants.WrongPassword);
        await HttpClient.JoinTableAuthorizedAsync<HttpResponseMessage>(createdTable.Id, joinRequest, joinerToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTableAuthorized_WithNonExistentTable_ReturnsNotFound()
    {
        // Arrange
        var (joinerToken, _, _) = await HttpClient.RegisterAndGetTokenAsync("auth_joiner_notfound", "Joiner@12345", "authnotfound");
        var nonExistentTableId = Guid.NewGuid();

        // Act
        var joinRequest = TableJoinHelpers.CreateJoinTableAuthorizedRequest(TestConstants.DefaultTablePassword);
        await HttpClient.JoinTableAuthorizedAsync<HttpResponseMessage>(nonExistentTableId, joinRequest, joinerToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTableAuthorized_WhenAlreadyMember_ReturnsConflict()
    {
        // Arrange - Create table and join with same user
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("auth_same_user", "Creator@12345", "authsame");

        var tableName = $"Already Member Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Act - Try to join the table they already created/are member of
        var joinRequest = TableJoinHelpers.CreateJoinTableAuthorizedRequest(TestConstants.DefaultTablePassword);
        await HttpClient.JoinTableAuthorizedAsync<HttpResponseMessage>(createdTable.Id, joinRequest, creatorToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTableAuthorized_WithFullTable_ReturnsBadRequest()
    {
        // Arrange - Create table with maxPlayers = 1 (only creator)
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("auth_full_creator", "Creator@12345", "authfull");

        var tableName = $"Full Table Auth {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName, maxPlayers: 1);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Register another user
        var (joinerToken, _, _) = await HttpClient.RegisterAndGetTokenAsync("auth_full_joiner", "Joiner@12345", "authfull");

        // Act - Try to join full table
        var joinRequest = TableJoinHelpers.CreateJoinTableAuthorizedRequest(TestConstants.DefaultTablePassword);
        await HttpClient.JoinTableAuthorizedAsync<HttpResponseMessage>(createdTable.Id, joinRequest, joinerToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTableAuthorized_MultipleUsersJoinSameTable_AllSucceed()
    {
        // Arrange - Create table
        var tableName = $"Multi Join Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, tableName: tableName, maxPlayers: 5);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Register multiple users
        var (joiner1Token, _, joiner1Login) = await HttpClient.RegisterAndGetTokenAsync("multi_joiner1", "Joiner1@12345", "multiauth");
        var (joiner2Token, _, joiner2Login) = await HttpClient.RegisterAndGetTokenAsync("multi_joiner2", "Joiner2@12345", "multiauth");
        var (joiner3Token, _, joiner3Login) = await HttpClient.RegisterAndGetTokenAsync("multi_joiner3", "Joiner3@12345", "multiauth");

        // Act - All three users join the table
        var joinRequest = TableJoinHelpers.CreateJoinTableAuthorizedRequest(TestConstants.DefaultTablePassword);
        await HttpClient.JoinTableAuthorizedAsync<HttpResponseMessage>(createdTable.Id, joinRequest, joiner1Token);
        await HttpClient.JoinTableAuthorizedAsync<HttpResponseMessage>(createdTable.Id, joinRequest, joiner2Token);
        await HttpClient.JoinTableAuthorizedAsync<HttpResponseMessage>(createdTable.Id, joinRequest, joiner3Token);

        // Assert - Get table details to verify all joined
        var tableDetails = await HttpClient.GetTableDetailsAsync<TableResponse>(createdTable.Id, joiner1Token);

        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTableAuthorized_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var (joinerToken, _, _) = await HttpClient.RegisterAndGetTokenAsync("auth_invalid_req", "Joiner@12345", "authinvalid");
        var tableId = Guid.NewGuid();

        // Act - Send request with empty password
        var emptyRequest = new JoinTableAuthorizedRequest { Password = "" };
        await HttpClient.JoinTableAuthorizedAsync<HttpResponseMessage>(tableId, emptyRequest, joinerToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTableAuthorized_WithValidPassword_ReturnsCompleteMemberInfo()
    {
        // Arrange
        var tableName = $"Member Info Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, tableName: tableName, stake: 100m, maxPlayers: 20);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        var (joinerToken, joinerUserId, joinerLogin) = await HttpClient.RegisterAndGetTokenAsync("auth_member_info", "Joiner@12345", "authmemberinfo");

        // Act
        var joinRequest = new JoinTableAuthorizedRequest { Password = TestConstants.DefaultTablePassword };
        var response = await HttpClient.JoinTableAuthorizedAsync<HttpResponseMessage>(createdTable.Id, joinRequest, joinerToken);

        var responseBody = await response.Content.ReadAsStringAsync();
        var joinResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<JoinTableResponse>(responseBody);

        // Assert - Verify response contains all required fields
        await VerifyHttpRecording();
    }

    #endregion

    #region Leaderboard Tests

    /// <summary>
    ///     Tests for the leaderboard functionality in the dashboard endpoint.
    /// </summary>

    [Fact]
    public async Task GetTableDashboard_WithExactHit_ReturnsLeaderboardWithThreePoints()
    {
        // Arrange - Create table with creator
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("lb_exact_creator", "Creator@12345", "lbexact");

        var tableName = $"Leaderboard Exact Hit Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Create match with past date (already started)
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Poland", "Spain", DateTime.UtcNow.AddMinutes(-10));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Place bet with exact prediction
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match.Id, new PlaceBetRequest { Prediction = "2:1" }, creatorToken);

        // Set match result (exact match)
        await HttpClient.SetMatchResultAsync<HttpResponseMessage>(match.Id, MatchResultHelpers.CreateSetResultRequest("2:1"), superAdminToken);

        // Act - Get dashboard
        await HttpClient.GetTableDashboardAsync<HttpResponseMessage>(createdTable.Id, creatorToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_WithWinnerHit_ReturnsLeaderboardWithOnePoint()
    {
        // Arrange - Create table with creator
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("lb_winner_creator", "Creator@12345", "lbwinner");

        var tableName = $"Leaderboard Winner Hit Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Create match with past date
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Germany", "France", DateTime.UtcNow.AddMinutes(-10));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Place bet - correct winner but wrong score
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match.Id, new PlaceBetRequest { Prediction = "2:0" }, creatorToken);

        // Set match result - same winner but different score
        await HttpClient.SetMatchResultAsync<HttpResponseMessage>(match.Id, MatchResultHelpers.CreateSetResultRequest("1:0"), superAdminToken);

        // Act - Get dashboard
        await HttpClient.GetTableDashboardAsync<HttpResponseMessage>(createdTable.Id, creatorToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_WithMiss_ReturnsLeaderboardWithZeroPoints()
    {
        // Arrange - Create table with creator
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("lb_miss_creator", "Creator@12345", "lbmiss");

        var tableName = $"Leaderboard Miss Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Create match with past date
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Italy", "Portugal", DateTime.UtcNow.AddMinutes(-10));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Place bet - home win prediction
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match.Id, new PlaceBetRequest { Prediction = "2:0" }, creatorToken);

        // Set match result - away win (miss)
        await HttpClient.SetMatchResultAsync<HttpResponseMessage>(match.Id, MatchResultHelpers.CreateSetResultRequest("0:1"), superAdminToken);

        // Act - Get dashboard
        await HttpClient.GetTableDashboardAsync<HttpResponseMessage>(createdTable.Id, creatorToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_WithDrawPredictionX_ReturnsWinnerHit()
    {
        // Arrange - Create table with creator
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("lb_draw_creator", "Creator@12345", "lbdraw");

        var tableName = $"Leaderboard Draw Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Create match with past date
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("England", "Scotland", DateTime.UtcNow.AddMinutes(-10));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Place bet with "X" for draw
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match.Id, new PlaceBetRequest { Prediction = "X" }, creatorToken);

        // Set match result - draw
        await HttpClient.SetMatchResultAsync<HttpResponseMessage>(match.Id, MatchResultHelpers.CreateSetResultRequest("1:1"), superAdminToken);

        // Act - Get dashboard
        await HttpClient.GetTableDashboardAsync<HttpResponseMessage>(createdTable.Id, creatorToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_WithMultipleUsers_ReturnsSortedLeaderboard()
    {
        // Arrange - Create table with multiple members
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("lb_multi_creator", "Creator@12345", "lbmulti");
        var (member1Token, _, member1Login) = await HttpClient.RegisterAndGetTokenAsync("lb_multi_member1", "Member1@12345", "lbmulti");
        var (member2Token, _, member2Login) = await HttpClient.RegisterAndGetTokenAsync("lb_multi_member2", "Member2@12345", "lbmulti");

        var tableName = $"Leaderboard Multi Users Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        await HttpClient.JoinTableAsExistingUserAsync<HttpResponseMessage>(tableName, "TablePass@123", member1Login, "Member1@12345");
        await HttpClient.JoinTableAsExistingUserAsync<HttpResponseMessage>(tableName, "TablePass@123", member2Login, "Member2@12345");

        // Create matches with past date
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var match1Request = EventTypeMatchCreation.CreateMatchRequest("Brazil", "Argentina", DateTime.UtcNow.AddMinutes(-10));
        var match2Request = EventTypeMatchCreation.CreateMatchRequest("Chile", "Uruguay", DateTime.UtcNow.AddMinutes(-10));
        var match1 = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, match1Request, superAdminToken);
        var match2 = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, match2Request, superAdminToken);

        // Creator: 2 exact hits = 6 points
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match1.Id, new PlaceBetRequest { Prediction = "2:1" }, creatorToken);
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match2.Id, new PlaceBetRequest { Prediction = "0:0" }, creatorToken);

        // Member1: 1 exact hit + 1 winner hit = 4 points
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match1.Id, new PlaceBetRequest { Prediction = "2:1" }, member1Token);
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match2.Id, new PlaceBetRequest { Prediction = "1:1" }, member1Token);

        // Member2: 2 misses = 0 points
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match1.Id, new PlaceBetRequest { Prediction = "0:2" }, member2Token);
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match2.Id, new PlaceBetRequest { Prediction = "2:0" }, member2Token);

        // Set match results
        await HttpClient.SetMatchResultAsync<HttpResponseMessage>(match1.Id, MatchResultHelpers.CreateSetResultRequest("2:1"), superAdminToken);
        await HttpClient.SetMatchResultAsync<HttpResponseMessage>(match2.Id, MatchResultHelpers.CreateSetResultRequest("0:0"), superAdminToken);

        // Act - Get dashboard
        await HttpClient.GetTableDashboardAsync<HttpResponseMessage>(createdTable.Id, creatorToken);

        // Assert - Leaderboard should be sorted: creator (6pts), member1 (4pts), member2 (0pts)
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_WithNoFinishedMatches_ReturnsEmptyLeaderboard()
    {
        // Arrange - Create table with members
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("lb_empty_creator", "Creator@12345", "lbempty");

        var tableName = $"Leaderboard Empty Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Create match with future date (not started)
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Japan", "Korea", DateTime.UtcNow.AddDays(1));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Place bet on unfinished match
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match.Id, new PlaceBetRequest { Prediction = "1:0" }, creatorToken);

        // Act - Get dashboard (no result set - match not finished)
        await HttpClient.GetTableDashboardAsync<HttpResponseMessage>(createdTable.Id, creatorToken);

        // Assert - Leaderboard should be empty since no matches have results
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_WithMultipleBets_CalculatesAccuracyCorrectly()
    {
        // Arrange - Create table with creator
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("lb_accuracy_creator", "Creator@12345", "lbaccuracy");

        var tableName = $"Leaderboard Accuracy Table {Guid.NewGuid()}";
        var createRequest = TableCreationHelpers.CreateDefaultTableRequest(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);
        var createdTable = await HttpClient.CreateTableAsync<TableResponse>(createRequest);

        // Create 5 matches with past dates
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var match1 = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId,
            EventTypeMatchCreation.CreateMatchRequest("Match1 Team A", "Match1 Team B", DateTime.UtcNow.AddMinutes(-10)), superAdminToken);
        var match2 = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId,
            EventTypeMatchCreation.CreateMatchRequest("Match2 Team A", "Match2 Team B", DateTime.UtcNow.AddMinutes(-10)), superAdminToken);
        var match3 = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId,
            EventTypeMatchCreation.CreateMatchRequest("Match3 Team A", "Match3 Team B", DateTime.UtcNow.AddMinutes(-10)), superAdminToken);
        var match4 = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId,
            EventTypeMatchCreation.CreateMatchRequest("Match4 Team A", "Match4 Team B", DateTime.UtcNow.AddMinutes(-10)), superAdminToken);
        var match5 = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId,
            EventTypeMatchCreation.CreateMatchRequest("Match5 Team A", "Match5 Team B", DateTime.UtcNow.AddMinutes(-10)), superAdminToken);

        // Place bets: 2 exact hits, 1 winner hit, 2 misses = 60% accuracy
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match1.Id, new PlaceBetRequest { Prediction = "2:1" }, creatorToken); // Exact hit
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match2.Id, new PlaceBetRequest { Prediction = "1:0" }, creatorToken); // Exact hit
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match3.Id, new PlaceBetRequest { Prediction = "3:0" }, creatorToken); // Winner hit
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match4.Id, new PlaceBetRequest { Prediction = "2:0" }, creatorToken); // Miss
        await HttpClient.PlaceBetAsync<dynamic>(createdTable.Id, match5.Id, new PlaceBetRequest { Prediction = "0:1" }, creatorToken); // Miss

        // Set match results
        await HttpClient.SetMatchResultAsync<HttpResponseMessage>(match1.Id, MatchResultHelpers.CreateSetResultRequest("2:1"), superAdminToken); // Exact
        await HttpClient.SetMatchResultAsync<HttpResponseMessage>(match2.Id, MatchResultHelpers.CreateSetResultRequest("1:0"), superAdminToken); // Exact
        await HttpClient.SetMatchResultAsync<HttpResponseMessage>(match3.Id, MatchResultHelpers.CreateSetResultRequest("2:0"), superAdminToken); // Winner
        await HttpClient.SetMatchResultAsync<HttpResponseMessage>(match4.Id, MatchResultHelpers.CreateSetResultRequest("0:0"), superAdminToken); // Miss
        await HttpClient.SetMatchResultAsync<HttpResponseMessage>(match5.Id, MatchResultHelpers.CreateSetResultRequest("1:0"), superAdminToken); // Miss

        // Act - Get dashboard
        await HttpClient.GetTableDashboardAsync<HttpResponseMessage>(createdTable.Id, creatorToken);

        // Assert - Accuracy should be 60% (3 correct / 5 total)
        // Points: 2×3 + 1×1 = 7, ExactHits: 2, WinnerHits: 1, TotalBets: 5, Accuracy: 60.0
        await VerifyHttpRecording();
    }

    #endregion
}
