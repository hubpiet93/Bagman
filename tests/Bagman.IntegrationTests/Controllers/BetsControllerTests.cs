using Bagman.Contracts.Models.Tables;
using Bagman.IntegrationTests.Controllers.Endpoints;
using Bagman.IntegrationTests.Controllers.Endpoints.Bets;
using Bagman.IntegrationTests.Controllers.Endpoints.EventTypes;
using Bagman.IntegrationTests.Controllers.Endpoints.Tables;
using Bagman.IntegrationTests.Helpers;
using Bagman.IntegrationTests.TestFixtures;
using Xunit.Abstractions;

namespace Bagman.IntegrationTests.Controllers;

[CollectionDefinition("Bets Tests")]
public class BetsTestsCollection : ICollectionFixture<PostgresFixture>
{
}

/// <summary>
///     Integration tests for BetsController actions using TestContainers for PostgreSQL.
///     Tests Place Bet, Get User Bet, and Delete Bet operations.
///     Bets are per user-match combination with unique constraint enforcement.
/// </summary>
[Collection("Bets Tests")]
public class BetsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    public BetsControllerTests(PostgresFixture postgresFixture, ITestOutputHelper testOutputHelper) : base(postgresFixture, testOutputHelper)
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
    public async Task PlaceBet_WithValidPrediction_ReturnsCreatedWithBetResponse()
    {
        // Arrange
        var (tableId, creatorToken, creatorLogin) = await HttpClient.CreateTableAsync(DefaultEventTypeId, "bet_creator");
        var (playerToken, _, playerLogin) = await HttpClient.RegisterAndGetTokenAsync("bet_player", TestConstants.DefaultUserPassword);
        // Get the table details to retrieve the table name
        var tableDetails = await HttpClient.GetTableDetailsAsync<TableResponse>(tableId, creatorToken);
        await HttpClient.JoinTableAsExistingUserAsync<dynamic>(tableDetails.Name, TestConstants.DefaultTablePassword, playerLogin, TestConstants.DefaultUserPassword);
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Italy", "France", DateTime.UtcNow.AddDays(5));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Act
        await HttpClient.PlaceBetAsync<HttpResponseMessage>(tableId, match.Id, new PlaceBetRequest {Prediction = "2:1"}, playerToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task PlaceBet_WithInvalidFormat_ReturnsBadRequest()
    {
        // Arrange
        var (tableId, creatorToken, _) = await HttpClient.CreateTableAsync(DefaultEventTypeId, "bet_invalid_creator");
        var (playerToken, _, playerLogin) = await HttpClient.RegisterAndGetTokenAsync("bet_invalid_player", TestConstants.DefaultUserPassword);
        var tableDetails = await HttpClient.GetTableDetailsAsync<TableResponse>(tableId, creatorToken);
        await HttpClient.JoinTableAsExistingUserAsync<dynamic>(tableDetails.Name, TestConstants.DefaultTablePassword, playerLogin, TestConstants.DefaultUserPassword);
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Italy", "France", DateTime.UtcNow.AddDays(5));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Act
        await HttpClient.PlaceBetAsync<HttpResponseMessage>(tableId, match.Id, new PlaceBetRequest {Prediction = "invalid prediction format"}, playerToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task PlaceBet_WithDrawPrediction_ReturnsCreated()
    {
        // Arrange
        var (tableId, creatorToken, _) = await HttpClient.CreateTableAsync(DefaultEventTypeId, "bet_draw_creator");
        var (playerToken, _, playerLogin) = await HttpClient.RegisterAndGetTokenAsync("bet_draw_player", TestConstants.DefaultUserPassword);
        var tableDetails = await HttpClient.GetTableDetailsAsync<TableResponse>(tableId, creatorToken);
        await HttpClient.JoinTableAsExistingUserAsync<dynamic>(tableDetails.Name, TestConstants.DefaultTablePassword, playerLogin, TestConstants.DefaultUserPassword);
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Italy", "France", DateTime.UtcNow.AddDays(5));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Act
        await HttpClient.PlaceBetAsync<HttpResponseMessage>(tableId, match.Id, new PlaceBetRequest {Prediction = "X"}, playerToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task PlaceBet_UpdateExistingBet_ReturnsOkWithUpdatedPrediction()
    {
        // Arrange
        var (tableId, creatorToken, _) = await HttpClient.CreateTableAsync(DefaultEventTypeId, "bet_update_creator");
        var (playerToken, _, playerLogin) = await HttpClient.RegisterAndGetTokenAsync("bet_update_player", TestConstants.DefaultUserPassword);
        var tableDetails = await HttpClient.GetTableDetailsAsync<TableResponse>(tableId, creatorToken);
        await HttpClient.JoinTableAsExistingUserAsync<dynamic>(tableDetails.Name, TestConstants.DefaultTablePassword, playerLogin, TestConstants.DefaultUserPassword);
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Italy", "France", DateTime.UtcNow.AddDays(5));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Place initial bet
        await HttpClient.PlaceBetAsync<HttpResponseMessage>(tableId, match.Id, new PlaceBetRequest {Prediction = "1:0"}, playerToken);

        // Act - Update bet
        await HttpClient.PlaceBetAsync<HttpResponseMessage>(tableId, match.Id, new PlaceBetRequest {Prediction = "2:1"}, playerToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetUserBet_WithExistingBet_ReturnsOkWithBetResponse()
    {
        // Arrange
        var (tableId, creatorToken, _) = await HttpClient.CreateTableAsync(DefaultEventTypeId, "bet_get_creator");
        var (playerToken, _, playerLogin) = await HttpClient.RegisterAndGetTokenAsync("bet_get_player", TestConstants.DefaultUserPassword);
        var tableDetails = await HttpClient.GetTableDetailsAsync<TableResponse>(tableId, creatorToken);
        await HttpClient.JoinTableAsExistingUserAsync<dynamic>(tableDetails.Name, TestConstants.DefaultTablePassword, playerLogin, TestConstants.DefaultUserPassword);
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Italy", "France", DateTime.UtcNow.AddDays(5));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        await HttpClient.PlaceBetAsync<HttpResponseMessage>(tableId, match.Id, new PlaceBetRequest {Prediction = "3:2"}, playerToken);

        // Act
        await HttpClient.GetUserBetAsync<HttpResponseMessage>(tableId, match.Id, playerToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetUserBet_WithoutBet_ReturnsNotFound()
    {
        // Arrange
        var (tableId, creatorToken, _) = await HttpClient.CreateTableAsync(DefaultEventTypeId, "bet_notfound_creator");
        var (playerToken, _, playerLogin) = await HttpClient.RegisterAndGetTokenAsync("bet_notfound_player", TestConstants.DefaultUserPassword);
        var tableDetails = await HttpClient.GetTableDetailsAsync<TableResponse>(tableId, creatorToken);
        await HttpClient.JoinTableAsExistingUserAsync<dynamic>(tableDetails.Name, TestConstants.DefaultTablePassword, playerLogin, TestConstants.DefaultUserPassword);
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Italy", "France", DateTime.UtcNow.AddDays(5));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Act
        await HttpClient.GetUserBetAsync<HttpResponseMessage>(tableId, match.Id, playerToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task DeleteBet_BeforeMatchStarted_ReturnsOk()
    {
        // Arrange
        var (tableId, creatorToken, _) = await HttpClient.CreateTableAsync(DefaultEventTypeId, "bet_delete_creator");
        var (playerToken, _, playerLogin) = await HttpClient.RegisterAndGetTokenAsync("bet_delete_player", TestConstants.DefaultUserPassword);
        var tableDetails = await HttpClient.GetTableDetailsAsync<TableResponse>(tableId, creatorToken);
        await HttpClient.JoinTableAsExistingUserAsync<dynamic>(tableDetails.Name, TestConstants.DefaultTablePassword, playerLogin, TestConstants.DefaultUserPassword);
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Italy", "France", DateTime.UtcNow.AddDays(5));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        await HttpClient.PlaceBetAsync<HttpResponseMessage>(tableId, match.Id, new PlaceBetRequest {Prediction = "1:1"}, playerToken);

        // Act
        await HttpClient.DeleteBetAsync<HttpResponseMessage>(tableId, match.Id, playerToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task DeleteBet_WithoutPlacedBet_ReturnsNotFound()
    {
        // Arrange
        var (tableId, creatorToken, _) = await HttpClient.CreateTableAsync(DefaultEventTypeId, "bet_delete_notfound_creator");
        var (playerToken, _, playerLogin) = await HttpClient.RegisterAndGetTokenAsync("bet_delete_notfound_player", TestConstants.DefaultUserPassword);
        var tableDetails = await HttpClient.GetTableDetailsAsync<TableResponse>(tableId, creatorToken);
        await HttpClient.JoinTableAsExistingUserAsync<dynamic>(tableDetails.Name, TestConstants.DefaultTablePassword, playerLogin, TestConstants.DefaultUserPassword);
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Italy", "France", DateTime.UtcNow.AddDays(5));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Act
        await HttpClient.DeleteBetAsync<HttpResponseMessage>(tableId, match.Id, playerToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task PlaceBet_MultipleUsersSameBet_BothSucceed()
    {
        // Arrange
        // Create table as creator
        var (tableId, creatorToken, creatorLogin) = await HttpClient.CreateTableAsync(DefaultEventTypeId,
            "bet_multi_creator");

        // Register two players
        var (player1Token, _, player1Login) = await HttpClient.RegisterAndGetTokenAsync("bet_multi_player1", TestConstants.DefaultUserPassword);
        var (player2Token, _, player2Login) = await HttpClient.RegisterAndGetTokenAsync("bet_multi_player2", TestConstants.DefaultUserPassword);

        // Get table details to join
        var tableDetails = await HttpClient.GetTableDetailsAsync<TableResponse>(tableId, creatorToken);

        // Both players join table
        await HttpClient.JoinTableAsExistingUserAsync<dynamic>(tableDetails.Name, TestConstants.DefaultTablePassword, player1Login, TestConstants.DefaultUserPassword);
        await HttpClient.JoinTableAsExistingUserAsync<dynamic>(tableDetails.Name, TestConstants.DefaultTablePassword, player2Login, TestConstants.DefaultUserPassword);

        // Create match
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Japan", "Korea", DateTime.UtcNow.AddDays(7));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Act - Both players place same bet
        await HttpClient.PlaceBetAsync<HttpResponseMessage>(tableId, match.Id, new PlaceBetRequest {Prediction = "1:1"}, player1Token);
        await HttpClient.PlaceBetAsync<HttpResponseMessage>(tableId, match.Id, new PlaceBetRequest {Prediction = "1:1"}, player2Token);

        // Assert
        await VerifyHttpRecording();
    }
}
