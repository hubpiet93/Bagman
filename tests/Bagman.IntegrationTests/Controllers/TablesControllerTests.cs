using Bagman.Contracts.Models.Tables;
using Bagman.IntegrationTests.Controllers.Endpoints;
using Bagman.IntegrationTests.Controllers.Endpoints.Bets;
using Bagman.IntegrationTests.Controllers.Endpoints.EventTypes;
using Bagman.IntegrationTests.Helpers;
using Bagman.IntegrationTests.TestFixtures;
using Xunit.Abstractions;
using static Bagman.IntegrationTests.Controllers.Endpoints.Tables.TableCreationHelpers;
using static Bagman.IntegrationTests.Controllers.Endpoints.Tables.TableJoinHelpers;
using static Bagman.IntegrationTests.Controllers.Endpoints.Tables.TableMembershipHelpers;
using static Bagman.IntegrationTests.Controllers.Endpoints.Tables.TableQueryHelpers;

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
    public async Task CreateTable_WithValidRequest_ReturnsOkWithTableResponse()
    {
        // Act
        await HttpClient.CreateTableNoRegisterAsync(DefaultEventTypeId);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateTable_WithInvalidTableName_ReturnsBadRequest()
    {
        // Act
        await HttpClient.CreateTableInvalidAsync(DefaultEventTypeId, "empty_name");

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateTable_WithNegativeStake_ReturnsBadRequest()
    {
        // Act
        await HttpClient.CreateTableInvalidAsync(DefaultEventTypeId, "negative_stake");

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTable_WithValidRequest_ReturnsOkWithTableResponse()
    {
        // Arrange
        var tableName = $"Joinable Table {Guid.NewGuid()}";
        await HttpClient.CreateTableNoRegisterAsync(DefaultEventTypeId, tableName: tableName);

        // Act
        await HttpClient.JoinTableNoRegisterAsync(tableName, "JoinPass@123");

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTable_WithWrongPassword_ReturnsBadRequest()
    {
        // Arrange
        var tableName = $"Protected Table {Guid.NewGuid()}";
        await HttpClient.CreateTableNoRegisterAsync(DefaultEventTypeId, tableName: tableName, userLogin: AuthEndpointsHelpers.Unique("creator_wrong_pass"));

        // Act
        await HttpClient.JoinTableAsync(tableName, "123");

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTable_WithFullTable_ReturnsBadRequest()
    {
        // Arrange
        var tableName = $"Full Table {Guid.NewGuid()}";
        await HttpClient.CreateTableNoRegisterAsync(DefaultEventTypeId, tableName: tableName, maxPlayers: 1);

        // Act
        await HttpClient.JoinTableNoRegisterAsync(tableName, "FullPass@123");

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetUserTables_WithMultipleTables_ReturnsOkWithTableList()
    {
        // Arrange - Register user and create multiple tables
        var (token, _, login) = await HttpClient.RegisterAndGetTokenAsync("multi_tables_user", "Pass@12345", "multi");

        await HttpClient.CreateAuthorizedTableAsync(DefaultEventTypeId, token);
        await HttpClient.CreateAuthorizedTableAsync(DefaultEventTypeId, token);

        // Act
        await HttpClient.GetUserTablesAsync(token);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDetails_WithValidId_ReturnsOkWithTableAndMembers()
    {
        // Arrange - Create table and join with another user
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("details_creator", "Creator@12345", "details");

        var tableName = $"Details Table {Guid.NewGuid()}";
        var createdTable = await HttpClient.CreateTableAsync(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);

        await HttpClient.JoinTableNoRegisterAsync(tableName, "DetailsPass@123");

        // Act
        await HttpClient.GetTableDetailsAsync(createdTable.Id, creatorToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task LeaveTable_AsRegularMember_ReturnsOk()
    {
        // Arrange - Create table and join with another user
        var tableName = $"Leave Table {Guid.NewGuid()}";
        var createdTable = await HttpClient.CreateTableAsync(DefaultEventTypeId, tableName: tableName);
        var (leaveToken, _, _) = await HttpClient.JoinTableAsNewUserAsync(tableName, "LeavePass@123");

        // Act
        await HttpClient.LeaveTableAsync(createdTable.Id, leaveToken);

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
        var createdTable = await HttpClient.CreateTableAsync(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);

        // Join with member
        await HttpClient.JoinTableAsExistingUserAsync(tableName, "TablePass@123", memberLogin, "Member@12345");

        // Act - Grant admin
        await HttpClient.GrantAdminAsync(createdTable.Id, memberId, creatorToken);

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
        var createdTable = await HttpClient.CreateTableAsync(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);

        // Join with member
        await HttpClient.JoinTableAsExistingUserAsync(tableName, "TablePass@123", memberLogin, "Member@12345");

        // Grant admin first
        await HttpClient.GrantAdminAsync(createdTable.Id, memberId, creatorToken);

        // Act - Revoke admin
        await HttpClient.RevokeAdminAsync(createdTable.Id, memberId, creatorToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task AuthorizedCreateTable_WithValidTokenAndRequest_ReturnsCreated()
    {
        // Arrange
        var (token, _, _) = await HttpClient.RegisterAndGetTokenAsync("auth_create_user", "Pass@12345", "authcreate");
        var request = CreateDefaultAuthorizedTableRequest(DefaultEventTypeId);

        // Act
        await HttpClient.CreateAuthorizedTableAsync(request, token);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task AuthorizedCreateTable_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var request = CreateDefaultAuthorizedTableRequest(DefaultEventTypeId);

        // Act
        await HttpClient.CreateAuthorizedTableNoTokenAsync(request);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task AuthorizedCreateTable_WithDuplicateName_ReturnsConflict409()
    {
        // Arrange
        var (token, _, _) = await HttpClient.RegisterAndGetTokenAsync("dup_create_user", "Pass@12345", "dupcreate");
        var tableName = $"Duplicate Name Table {Guid.NewGuid()}";
        
        var first = CreateDefaultAuthorizedTableRequest(DefaultEventTypeId, tableName);
        await HttpClient.CreateAuthorizedTableAsync(first, token);

        var second = CreateDefaultAuthorizedTableRequest(DefaultEventTypeId, tableName);

        // Act
        await HttpClient.CreateAuthorizedTableAsync(second, token);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task AuthorizedCreateTable_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var (token, _, _) = await HttpClient.RegisterAndGetTokenAsync("invalid_create_user", "Pass@12345", "invalidcreate");

        // Act
        await HttpClient.CreateAuthorizedTableInvalidAsync(DefaultEventTypeId, "empty_name", token);

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
        var createdTable = await HttpClient.CreateTableAsync(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);

        // Join table with member
        await HttpClient.JoinTableAsExistingUserAsync(tableName, "TablePass@123", memberLogin, "Member@12345");

        // Act - Get dashboard as member
        await HttpClient.GetTableDashboardAsync(createdTable.Id, memberToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var tableId = Guid.NewGuid();

        // Act - Try to get dashboard without token
        await HttpClient.GetTableDashboardWithoutTokenAsync(tableId);

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
        var createdTable = await HttpClient.CreateTableAsync(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);

        // Act - Try to get dashboard as non-member
        await HttpClient.GetTableDashboardAsync(createdTable.Id, nonMemberToken);

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
        await HttpClient.GetTableDashboardAsync(nonExistentTableId, token);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetTableDashboard_IncludesTableInfo_Members_AndEmptyData()
    {
        // Arrange - Create a table as the only member
        var (token, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("dash_solo", "Pass@12345", "dashsolo");

        var tableName = $"Solo Dashboard Table {Guid.NewGuid()}";
        var createdTable = await HttpClient.CreateTableAsync(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);

        // Act - Get dashboard should return table info with empty matches/bets/pools/stats
        await HttpClient.GetTableDashboardAsync(createdTable.Id, token);

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
        var createdTable = await HttpClient.CreateTableAsync(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);

        // Join table with members
        await HttpClient.JoinTableAsExistingUserAsync(tableName, "TablePass@123", member1Login, "Member1@12345");
        await HttpClient.JoinTableAsExistingUserAsync(tableName, "TablePass@123", member2Login, "Member2@12345");

        // Get SuperAdmin token and create matches
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var match1 = await HttpClient.CreateMatchAsync(DefaultEventTypeId, "Poland", "Spain", superAdminToken, DateTime.UtcNow.AddDays(1));
        var match2 = await HttpClient.CreateMatchAsync(DefaultEventTypeId, "Germany", "France", superAdminToken, DateTime.UtcNow.AddDays(2));

        // Place bets from different users
        await HttpClient.PlaceBetAsync(createdTable.Id, match1.Id, "1:0", creatorToken);
        await HttpClient.PlaceBetAsync(createdTable.Id, match1.Id, "2:1", member1Token);
        await HttpClient.PlaceBetAsync(createdTable.Id, match2.Id, "X", member2Token);

        // Act - Get dashboard as creator (should see all members and all bets)
        await HttpClient.GetTableDashboardAsync(createdTable.Id, creatorToken);

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
        var createdTable = await HttpClient.CreateTableAsync(DefaultEventTypeId, tableName: tableName);

        // Register and get token for the joining user
        var (joinerToken, joinerUserId, joinerLogin) = await HttpClient.RegisterAndGetTokenAsync("auth_joiner", "Joiner@12345", "authjoin");

        // Act
        await HttpClient.JoinTableAuthorizedAsync(createdTable.Id, TestConstants.DefaultTablePassword, joinerToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTableAuthorized_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange - Create table
        var tableName = $"Unauthorized Join Table {Guid.NewGuid()}";
        var createdTable = await HttpClient.CreateTableAsync(DefaultEventTypeId, tableName: tableName);

        // Act - Try to join without token
        await HttpClient.JoinTableAuthorizedWithoutTokenAsync(createdTable.Id, TestConstants.DefaultTablePassword);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTableAuthorized_WithWrongPassword_ReturnsForbidden()
    {
        // Arrange - Create table
        var tableName = $"Wrong Password Join Table {Guid.NewGuid()}";
        var createdTable = await HttpClient.CreateTableAsync(DefaultEventTypeId, tableName: tableName);

        // Register user with valid token
        var (joinerToken, _, _) = await HttpClient.RegisterAndGetTokenAsync("auth_joiner_wrong_pass", "Joiner@12345", "authwrongpass");

        // Act - Try to join with wrong password
        await HttpClient.JoinTableAuthorizedWithWrongPasswordAsync(createdTable.Id, joinerToken);

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
        await HttpClient.JoinTableAuthorizedAsync(nonExistentTableId, TestConstants.DefaultTablePassword, joinerToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTableAuthorized_WhenAlreadyMember_ReturnsConflict()
    {
        // Arrange - Create table and join with same user
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("auth_same_user", "Creator@12345", "authsame");
        
        var tableName = $"Already Member Table {Guid.NewGuid()}";
        var createdTable = await HttpClient.CreateTableAsync(DefaultEventTypeId, userLogin: creatorLogin, tableName: tableName);

        // Act - Try to join the table they already created/are member of
        await HttpClient.JoinTableAuthorizedAsync(createdTable.Id, TestConstants.DefaultTablePassword, creatorToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTableAuthorized_WithFullTable_ReturnsForbidden()
    {
        // Arrange - Create table with maxPlayers = 1 (only creator)
        var (creatorToken, _, creatorLogin) = await HttpClient.RegisterAndGetTokenAsync("auth_full_creator", "Creator@12345", "authfull");

        var tableName = $"Full Table Auth {Guid.NewGuid()}";
        var createdTable = await HttpClient.CreateTableAsync(
            DefaultEventTypeId,
            userLogin: creatorLogin,
            tableName: tableName,
            maxPlayers: 1);

        // Register another user
        var (joinerToken, _, _) = await HttpClient.RegisterAndGetTokenAsync("auth_full_joiner", "Joiner@12345", "authfull");

        // Act - Try to join full table
        await HttpClient.JoinTableAuthorizedAsync(createdTable.Id, TestConstants.DefaultTablePassword, joinerToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTableAuthorized_MultipleUsersJoinSameTable_AllSucceed()
    {
        // Arrange - Create table
        var tableName = $"Multi Join Table {Guid.NewGuid()}";
        var createdTable = await HttpClient.CreateTableAsync(DefaultEventTypeId, tableName: tableName, maxPlayers: 5);

        // Register multiple users
        var (joiner1Token, _, joiner1Login) = await HttpClient.RegisterAndGetTokenAsync("multi_joiner1", "Joiner1@12345", "multiauth");
        var (joiner2Token, _, joiner2Login) = await HttpClient.RegisterAndGetTokenAsync("multi_joiner2", "Joiner2@12345", "multiauth");
        var (joiner3Token, _, joiner3Login) = await HttpClient.RegisterAndGetTokenAsync("multi_joiner3", "Joiner3@12345", "multiauth");

        // Act - All three users join the table
        await HttpClient.JoinTableAuthorizedAsync(createdTable.Id, TestConstants.DefaultTablePassword, joiner1Token);
        await HttpClient.JoinTableAuthorizedAsync(createdTable.Id, TestConstants.DefaultTablePassword, joiner2Token);
        await HttpClient.JoinTableAuthorizedAsync(createdTable.Id, TestConstants.DefaultTablePassword, joiner3Token);

        // Assert - Get table details to verify all joined
        var tableDetails = await HttpClient.GetTableDetailsAsync(createdTable.Id, joiner1Token);
        
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
        await HttpClient.PostAsJsonWithoutDeserializeAsync($"/api/tables/{tableId}/join", emptyRequest, joinerToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task JoinTableAuthorized_WithValidPassword_ReturnsCompleteMemberInfo()
    {
        // Arrange
        var tableName = $"Member Info Table {Guid.NewGuid()}";
        var createdTable = await HttpClient.CreateTableAsync(
            DefaultEventTypeId,
            tableName: tableName,
            stake: 100m,
            maxPlayers: 20);

        var (joinerToken, joinerUserId, joinerLogin) = await HttpClient.RegisterAndGetTokenAsync("auth_member_info", "Joiner@12345", "authmemberinfo");

        // Act
        var response = await HttpClient.PostAsJsonWithoutDeserializeAsync(
            $"/api/tables/{createdTable.Id}/join",
            new JoinTableAuthorizedRequest { Password = TestConstants.DefaultTablePassword },
            joinerToken);

        var responseBody = await response.Content.ReadAsStringAsync();
        var joinResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<JoinTableResponse>(responseBody);

        // Assert - Verify response contains all required fields
        await VerifyHttpRecording();
    }

    #endregion
}
