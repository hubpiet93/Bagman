using System.Net.Http.Headers;
using System.Text;
using Bagman.Contracts.Models.Auth;
using Bagman.Contracts.Models.Tables;
using Bagman.IntegrationTests.TestFixtures;
using Newtonsoft.Json;

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
    public BetsControllerTests(PostgresFixture postgresFixture) : base(postgresFixture)
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

    private async Task<(Guid TableId, Guid MatchId, string CreatorToken, string PlayerToken, Guid PlayerId)> SetupTableWithMatch(
        string creatorLogin,
        string creatorPassword,
        string playerLogin,
        string playerPassword)
    {
        // Make logins unique to avoid conflicts when tests run against the same DB instance
        // Use shorter suffix to keep login length within validation limits
        var creatorLoginUnique = $"{creatorLogin}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        var playerLoginUnique = $"{playerLogin}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";

        // Create table
        var createTableRequest = new CreateTableRequest
        {
            UserLogin = creatorLoginUnique,
            UserPassword = creatorPassword,
            TableName = $"Betting Table {Guid.NewGuid()}",
            TablePassword = "BettingPass@123",
            MaxPlayers = 10,
            Stake = 50m
        };

        var createTableContent = new StringContent(
            JsonConvert.SerializeObject(createTableRequest),
            Encoding.UTF8,
            "application/json");

        var createTableResponse = await HttpClient!.PostAsync("/api/tables", createTableContent);
        var createTableBody = await createTableResponse.Content.ReadAsStringAsync();
        var tableResponse = JsonConvert.DeserializeObject<TableResponse>(createTableBody);

        var tableId = tableResponse!.Id;

        // After creating table, the API auto-registers the creator. Obtain creator token by logging in.
        var creatorLoginRequest = new LoginRequest
        {
            Login = creatorLoginUnique,
            Password = creatorPassword
        };

        var creatorLoginContent = new StringContent(
            JsonConvert.SerializeObject(creatorLoginRequest),
            Encoding.UTF8,
            "application/json");

        var creatorLoginResponse = await HttpClient.PostAsync("/api/auth/login", creatorLoginContent);
        var creatorLoginBody = await creatorLoginResponse.Content.ReadAsStringAsync();
        var creatorAuthResponse = JsonConvert.DeserializeObject<AuthResponse>(creatorLoginBody);
        var creatorToken = creatorAuthResponse!.AccessToken;

        // Register player to get token and id, then join the table
        var playerRegisterRequest = new RegisterRequest
        {
            Login = playerLoginUnique,
            Password = playerPassword,
            Email = $"{playerLoginUnique}@example.com"
        };

        var playerRegisterContent = new StringContent(
            JsonConvert.SerializeObject(playerRegisterRequest),
            Encoding.UTF8,
            "application/json");

        var playerRegisterResponse = await HttpClient.PostAsync("/api/auth/register", playerRegisterContent);
        var playerRegisterBody = await playerRegisterResponse.Content.ReadAsStringAsync();
        var playerAuthResponse = JsonConvert.DeserializeObject<AuthResponse>(playerRegisterBody);

        var playerToken = playerAuthResponse!.AccessToken;
        var playerId = playerAuthResponse.User.Id;

        var joinRequest = new JoinTableRequest
        {
            UserLogin = playerLoginUnique,
            UserPassword = playerPassword,
            TableName = tableResponse.Name,
            TablePassword = "BettingPass@123"
        };

        var joinContent = new StringContent(
            JsonConvert.SerializeObject(joinRequest),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/tables/join", joinContent);

        // Create match
        var createMatchRequest = new CreateMatchRequest
        {
            Country1 = "Italy",
            Country2 = "France",
            MatchDateTime = DateTime.UtcNow.AddDays(5)
        };

        var matchContent = new StringContent(
            JsonConvert.SerializeObject(createMatchRequest),
            Encoding.UTF8,
            "application/json");

        var createMatchHttpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches")
        {
            Content = matchContent
        };
        createMatchHttpRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", creatorToken);

        var createMatchResponse = await HttpClient.SendAsync(createMatchHttpRequest);
        var createMatchBody = await createMatchResponse.Content.ReadAsStringAsync();
        var matchResponse = JsonConvert.DeserializeObject<MatchResponse>(createMatchBody);

        return (tableId, matchResponse!.Id, creatorToken, playerToken, playerId);
    }

    [Fact]
    public async Task PlaceBet_WithValidPrediction_ReturnsCreatedWithBetResponse()
    {
        // Arrange
        var (tableId, matchId, _, playerToken, _) = await SetupTableWithMatch(
            "bet_creator", "Creator@12345",
            "bet_player", "Player@12345");

        var placeBetRequest = new PlaceBetRequest
        {
            Prediction = "2:1"
        };

        var betContent = new StringContent(
            JsonConvert.SerializeObject(placeBetRequest),
            Encoding.UTF8,
            "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches/{matchId}/bets")
        {
            Content = betContent
        };
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", playerToken);

        // Act
        await HttpClient!.SendAsync(request);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task PlaceBet_WithInvalidFormat_ReturnsBadRequest()
    {
        // Arrange
        var (tableId, matchId, _, playerToken, _) = await SetupTableWithMatch(
            "bet_invalid_creator", "Creator@12345",
            "bet_invalid_player", "Player@12345");

        var placeBetRequest = new PlaceBetRequest
        {
            Prediction = "invalid prediction format" // Invalid format
        };

        var betContent = new StringContent(
            JsonConvert.SerializeObject(placeBetRequest),
            Encoding.UTF8,
            "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches/{matchId}/bets")
        {
            Content = betContent
        };
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", playerToken);

        // Act
        await HttpClient!.SendAsync(request);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task PlaceBet_WithDrawPrediction_ReturnsOk()
    {
        // Arrange
        var (tableId, matchId, _, playerToken, _) = await SetupTableWithMatch(
            "bet_draw_creator", "Creator@12345",
            "bet_draw_player", "Player@12345");

        var placeBetRequest = new PlaceBetRequest
        {
            Prediction = "X" // Draw
        };

        var betContent = new StringContent(
            JsonConvert.SerializeObject(placeBetRequest),
            Encoding.UTF8,
            "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches/{matchId}/bets")
        {
            Content = betContent
        };
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", playerToken);

        // Act
        await HttpClient!.SendAsync(request);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task PlaceBet_UpdateExistingBet_ReturnsOkWithUpdatedPrediction()
    {
        // Arrange
        var (tableId, matchId, _, playerToken, _) = await SetupTableWithMatch(
            "bet_update_creator", "Creator@12345",
            "bet_update_player", "Player@12345");

        // Place initial bet
        var initialBetRequest = new PlaceBetRequest
        {
            Prediction = "1:0"
        };

        var initialBetContent = new StringContent(
            JsonConvert.SerializeObject(initialBetRequest),
            Encoding.UTF8,
            "application/json");

        var initialRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches/{matchId}/bets")
        {
            Content = initialBetContent
        };
        initialRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", playerToken);

        await HttpClient!.SendAsync(initialRequest);

        // Act - Update bet
        var updatedBetRequest = new PlaceBetRequest
        {
            Prediction = "2:1" // Changed prediction
        };

        var updatedBetContent = new StringContent(
            JsonConvert.SerializeObject(updatedBetRequest),
            Encoding.UTF8,
            "application/json");

        var updateRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches/{matchId}/bets")
        {
            Content = updatedBetContent
        };
        updateRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", playerToken);

        await HttpClient.SendAsync(updateRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetUserBet_WithExistingBet_ReturnsOkWithBetResponse()
    {
        // Arrange
        var (tableId, matchId, _, playerToken, _) = await SetupTableWithMatch(
            "bet_get_creator", "Creator@12345",
            "bet_get_player", "Player@12345");

        var placeBetRequest = new PlaceBetRequest
        {
            Prediction = "3:2"
        };

        var betContent = new StringContent(
            JsonConvert.SerializeObject(placeBetRequest),
            Encoding.UTF8,
            "application/json");

        var placeRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches/{matchId}/bets")
        {
            Content = betContent
        };
        placeRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", playerToken);

        await HttpClient!.SendAsync(placeRequest);

        // Act - Get bet
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/tables/{tableId}/matches/{matchId}/bets/my");
        getRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", playerToken);

        await HttpClient.SendAsync(getRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetUserBet_WithoutBet_ReturnsNotFound()
    {
        // Arrange
        var (tableId, matchId, _, playerToken, _) = await SetupTableWithMatch(
            "bet_notfound_creator", "Creator@12345",
            "bet_notfound_player", "Player@12345");

        // Act - Get bet without placing one
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/tables/{tableId}/matches/{matchId}/bets/my");
        getRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", playerToken);

        await HttpClient!.SendAsync(getRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task DeleteBet_BeforeMatchStarted_ReturnsOk()
    {
        // Arrange
        var (tableId, matchId, _, playerToken, _) = await SetupTableWithMatch(
            "bet_delete_creator", "Creator@12345",
            "bet_delete_player", "Player@12345");

        // Place bet
        var placeBetRequest = new PlaceBetRequest
        {
            Prediction = "1:1"
        };

        var betContent = new StringContent(
            JsonConvert.SerializeObject(placeBetRequest),
            Encoding.UTF8,
            "application/json");

        var placeRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches/{matchId}/bets")
        {
            Content = betContent
        };
        placeRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", playerToken);

        await HttpClient!.SendAsync(placeRequest);

        // Act - Delete bet
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/tables/{tableId}/matches/{matchId}/bets");
        deleteRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", playerToken);

        await HttpClient.SendAsync(deleteRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task DeleteBet_WithoutPlacedBet_ReturnsNotFound()
    {
        // Arrange
        var (tableId, matchId, _, playerToken, _) = await SetupTableWithMatch(
            "bet_delete_notfound_creator", "Creator@12345",
            "bet_delete_notfound_player", "Player@12345");

        // Act - Delete bet without placing one
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/tables/{tableId}/matches/{matchId}/bets");
        deleteRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", playerToken);

        await HttpClient!.SendAsync(deleteRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task PlaceBet_MultipleUsersSameBet_BothSucceed()
    {
        // Arrange
        var creatorLogin = "bet_multi_creator";
        var player1Login = "bet_multi_player1";
        var player2Login = "bet_multi_player2";

        // Register all users
        var creatorRegisterRequest = new RegisterRequest
        {
            Login = creatorLogin,
            Password = "Creator@12345",
            Email = $"{creatorLogin}@example.com"
        };

        var creatorRegisterContent = new StringContent(
            JsonConvert.SerializeObject(creatorRegisterRequest),
            Encoding.UTF8,
            "application/json");

        var creatorRegisterResponse = await HttpClient!.PostAsync("/api/auth/register", creatorRegisterContent);
        var creatorRegisterBody = await creatorRegisterResponse.Content.ReadAsStringAsync();
        var creatorAuthResponse = JsonConvert.DeserializeObject<AuthResponse>(creatorRegisterBody);
        var creatorToken = creatorAuthResponse!.AccessToken;

        var player1RegisterRequest = new RegisterRequest
        {
            Login = player1Login,
            Password = "Player1@12345",
            Email = $"{player1Login}@example.com"
        };

        var player1RegisterContent = new StringContent(
            JsonConvert.SerializeObject(player1RegisterRequest),
            Encoding.UTF8,
            "application/json");

        var player1RegisterResponse = await HttpClient.PostAsync("/api/auth/register", player1RegisterContent);
        var player1RegisterBody = await player1RegisterResponse.Content.ReadAsStringAsync();
        var player1AuthResponse = JsonConvert.DeserializeObject<AuthResponse>(player1RegisterBody);
        var player1Token = player1AuthResponse!.AccessToken;

        var player2RegisterRequest = new RegisterRequest
        {
            Login = player2Login,
            Password = "Player2@12345",
            Email = $"{player2Login}@example.com"
        };

        var player2RegisterContent = new StringContent(
            JsonConvert.SerializeObject(player2RegisterRequest),
            Encoding.UTF8,
            "application/json");

        var player2RegisterResponse = await HttpClient.PostAsync("/api/auth/register", player2RegisterContent);
        var player2RegisterBody = await player2RegisterResponse.Content.ReadAsStringAsync();
        var player2AuthResponse = JsonConvert.DeserializeObject<AuthResponse>(player2RegisterBody);
        var player2Token = player2AuthResponse!.AccessToken;

        // Create table
        var createTableRequest = new CreateTableRequest
        {
            UserLogin = creatorLogin,
            UserPassword = "Creator@12345",
            TableName = $"Multi Betting Table {Guid.NewGuid()}",
            TablePassword = "MultiPass@123",
            MaxPlayers = 10,
            Stake = 50m
        };

        var createTableContent = new StringContent(
            JsonConvert.SerializeObject(createTableRequest),
            Encoding.UTF8,
            "application/json");

        var createTableResponse = await HttpClient.PostAsync("/api/tables", createTableContent);
        var createTableBody = await createTableResponse.Content.ReadAsStringAsync();
        var tableResponse = JsonConvert.DeserializeObject<TableResponse>(createTableBody);

        var tableId = tableResponse!.Id;

        // Join table with both players
        var joinRequest1 = new JoinTableRequest
        {
            UserLogin = player1Login,
            UserPassword = "Player1@12345",
            TableName = tableResponse.Name,
            TablePassword = "MultiPass@123"
        };

        var joinContent1 = new StringContent(
            JsonConvert.SerializeObject(joinRequest1),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/tables/join", joinContent1);

        var joinRequest2 = new JoinTableRequest
        {
            UserLogin = player2Login,
            UserPassword = "Player2@12345",
            TableName = tableResponse.Name,
            TablePassword = "MultiPass@123"
        };

        var joinContent2 = new StringContent(
            JsonConvert.SerializeObject(joinRequest2),
            Encoding.UTF8,
            "application/json");

        await HttpClient.PostAsync("/api/tables/join", joinContent2);

        // Create match
        var createMatchRequest = new CreateMatchRequest
        {
            Country1 = "Japan",
            Country2 = "Korea",
            MatchDateTime = DateTime.UtcNow.AddDays(7)
        };

        var matchContent = new StringContent(
            JsonConvert.SerializeObject(createMatchRequest),
            Encoding.UTF8,
            "application/json");

        var createMatchHttpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches")
        {
            Content = matchContent
        };
        createMatchHttpRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", creatorToken);

        var createMatchResponse = await HttpClient.SendAsync(createMatchHttpRequest);
        var createMatchBody = await createMatchResponse.Content.ReadAsStringAsync();
        var matchResponse = JsonConvert.DeserializeObject<MatchResponse>(createMatchBody);

        var matchId = matchResponse!.Id;

        // Act - Both players place same bet
        var placeBetRequest = new PlaceBetRequest
        {
            Prediction = "1:1"
        };

        var bet1Request = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches/{matchId}/bets")
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(placeBetRequest),
                Encoding.UTF8,
                "application/json")
        };
        bet1Request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", player1Token);

        var bet2Request = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches/{matchId}/bets")
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(placeBetRequest),
                Encoding.UTF8,
                "application/json")
        };
        bet2Request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", player2Token);

        await HttpClient.SendAsync(bet1Request);
        await HttpClient.SendAsync(bet2Request);

        // Assert (snapshot includes: registrations + table create + joins + match create + both bets)
        await VerifyHttpRecording();
    }
}
