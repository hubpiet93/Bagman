using System.Net;
using System.Text;
using Newtonsoft.Json;
using Bagman.Contracts.Models.Auth;
using Bagman.Contracts.Models.Tables;
using Bagman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VerifyXunit;
using Xunit;

namespace Bagman.IntegrationTests.Controllers;

[CollectionDefinition("Bets Tests")]
public class BetsTestsCollection : ICollectionFixture<TestFixtures.PostgresFixture>
{
}

/// <summary>
/// Integration tests for BetsController actions using TestContainers for PostgreSQL.
/// Tests Place Bet, Get User Bet, and Delete Bet operations.
/// Bets are per user-match combination with unique constraint enforcement.
/// </summary>
[Collection("Bets Tests")]
public class BetsControllerTests : IAsyncLifetime
{
    private readonly TestFixtures.PostgresFixture _postgresFixture;
    private TestFixtures.AuthTestWebApplicationFactory? _factory;
    private HttpClient? _httpClient;
    private bool _initialized = false;

    public BetsControllerTests(TestFixtures.PostgresFixture postgresFixture)
    {
        _postgresFixture = postgresFixture;
    }

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        _initialized = true;

        if (_postgresFixture.ConnectionString == null)
        {
            await _postgresFixture.InitializeAsync();
        }

        _factory = new TestFixtures.AuthTestWebApplicationFactory(_postgresFixture.ConnectionString!);
        _httpClient = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        int maxRetries = 3;
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                await dbContext.Database.EnsureCreatedAsync();
                // Ensure database is clean between tests to allow deterministic logins
                await dbContext.Database.ExecuteSqlRawAsync(
                    "TRUNCATE TABLE pool_winners, pools, bets, matches, user_stats, table_members, tables, refresh_tokens, users RESTART IDENTITY CASCADE;");
                break;
            }
            catch (Exception) when (i < maxRetries - 1)
            {
                await Task.Delay(1000);
            }
        }
    }

    public async Task DisposeAsync()
    {
        _httpClient?.Dispose();
        _factory?.Dispose();
    }

    private async Task<(Guid TableId, Guid MatchId, string CreatorToken, string PlayerToken, Guid PlayerId)> SetupTableWithMatch(
        string creatorLogin, 
        string creatorPassword,
        string playerLogin, 
        string playerPassword)
    {
        // Make logins unique to avoid conflicts when tests run against the same DB instance
        // Use shorter suffix to keep login length within validation limits
        var creatorLoginUnique = $"{creatorLogin}_{Guid.NewGuid().ToString("N").Substring(0,8)}";
        var playerLoginUnique = $"{playerLogin}_{Guid.NewGuid().ToString("N").Substring(0,8)}";

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

        var createTableResponse = await _httpClient.PostAsync("/api/tables", createTableContent);
        var createTableBody = await createTableResponse.Content.ReadAsStringAsync();
        
        if (!createTableResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to create table: {createTableResponse.StatusCode} - {createTableBody}");
        
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

        var creatorLoginResponse = await _httpClient.PostAsync("/api/auth/login", creatorLoginContent);
        var creatorLoginBody = await creatorLoginResponse.Content.ReadAsStringAsync();
        if (!creatorLoginResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to login creator after table creation: {creatorLoginResponse.StatusCode} - {creatorLoginBody}");

        var creatorAuthResponse = JsonConvert.DeserializeObject<Contracts.Models.Auth.AuthResponse>(creatorLoginBody);
        var creatorToken = creatorAuthResponse!.AccessToken;

        // Join table as player
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

        var playerRegisterResponse = await _httpClient.PostAsync("/api/auth/register", playerRegisterContent);
        var playerRegisterBody = await playerRegisterResponse.Content.ReadAsStringAsync();
        
        if (!playerRegisterResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to register player: {playerRegisterResponse.StatusCode} - {playerRegisterBody}");
        
        var playerAuthResponse = JsonConvert.DeserializeObject<Contracts.Models.Auth.AuthResponse>(playerRegisterBody);

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

        var joinResp = await _httpClient.PostAsync("/api/tables/join", joinContent);
        var joinBody = await joinResp.Content.ReadAsStringAsync();
        if (!joinResp.IsSuccessStatusCode)
            throw new Exception($"Failed to join table as player: {joinResp.StatusCode} - {joinBody}");

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
        createMatchHttpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", creatorToken);

        var createMatchResponse = await _httpClient.SendAsync(createMatchHttpRequest);
        var createMatchBody = await createMatchResponse.Content.ReadAsStringAsync();
        
        if (!createMatchResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to create match: {createMatchResponse.StatusCode} - {createMatchBody}");
        
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
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", playerToken);

        // Act
        var response = await _httpClient!.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to place bet: {response.StatusCode} - {responseBody}");
        
        var betResponse = JsonConvert.DeserializeObject<BetResponse>(responseBody);

        // Assert
        await Verify(new
        {
            StatusCode = response.StatusCode,
            Bet = new
            {
                betResponse!.Prediction,
                betResponse.MatchId
            },
            HasId = betResponse.Id != Guid.Empty
        });
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
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", playerToken);

        // Act
        var response = await _httpClient!.SendAsync(request);

        // Assert
        await Verify(new
        {
            response.StatusCode,
            IsBadRequest = response.StatusCode == HttpStatusCode.BadRequest
        });
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
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", playerToken);

        // Act
        var response = await _httpClient!.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to place bet: {response.StatusCode} - {responseBody}");
        
        var betResponse = JsonConvert.DeserializeObject<BetResponse>(responseBody);

        // Assert
        await Verify(new
        {
            StatusCode = response.StatusCode,
            Bet = new
            {
                betResponse!.Prediction
            }
        });
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
        initialRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", playerToken);

        await _httpClient!.SendAsync(initialRequest);

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
        updateRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", playerToken);

        var response = await _httpClient.SendAsync(updateRequest);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to update bet: {response.StatusCode} - {responseBody}");
        
        var updatedBetResponse = JsonConvert.DeserializeObject<BetResponse>(responseBody);

        // Assert
        await Verify(new
        {
            StatusCode = response.StatusCode,
            Bet = new
            {
                updatedBetResponse!.Prediction
            }
        });
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
        placeRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", playerToken);

        await _httpClient!.SendAsync(placeRequest);

        // Act - Get bet
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/tables/{tableId}/matches/{matchId}/bets/my");
        getRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", playerToken);
        var response = await _httpClient.SendAsync(getRequest);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to get bet: {response.StatusCode} - {responseBody}");
        
        var betResponse = JsonConvert.DeserializeObject<BetResponse>(responseBody);

        // Assert
        await Verify(new
        {
            StatusCode = response.StatusCode,
            Bet = new
            {
                betResponse!.Prediction,
                betResponse.MatchId
            }
        });
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
        getRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", playerToken);
        var response = await _httpClient!.SendAsync(getRequest);

        // Assert
        await Verify(new
        {
            response.StatusCode,
            IsNotFound = response.StatusCode == HttpStatusCode.NotFound
        });
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
        placeRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", playerToken);

        await _httpClient!.SendAsync(placeRequest);

        // Act - Delete bet
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/tables/{tableId}/matches/{matchId}/bets");
        deleteRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", playerToken);
        var response = await _httpClient.SendAsync(deleteRequest);

        // Assert
        await Verify(new
        {
            StatusCode = response.StatusCode,
            IsOk = response.StatusCode == HttpStatusCode.OK
        });
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
        deleteRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", playerToken);
        var response = await _httpClient!.SendAsync(deleteRequest);

        // Assert
        await Verify(new
        {
            response.StatusCode,
            IsNotFound = response.StatusCode == HttpStatusCode.NotFound
        });
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

        var creatorRegisterResponse = await _httpClient!.PostAsync("/api/auth/register", creatorRegisterContent);
        var creatorRegisterBody = await creatorRegisterResponse.Content.ReadAsStringAsync();
        
        if (!creatorRegisterResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to register creator: {creatorRegisterResponse.StatusCode} - {creatorRegisterBody}");
        
        var creatorAuthResponse = JsonConvert.DeserializeObject<Contracts.Models.Auth.AuthResponse>(creatorRegisterBody);

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

        var player1RegisterResponse = await _httpClient.PostAsync("/api/auth/register", player1RegisterContent);
        var player1RegisterBody = await player1RegisterResponse.Content.ReadAsStringAsync();
        
        if (!player1RegisterResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to register player1: {player1RegisterResponse.StatusCode} - {player1RegisterBody}");
        
        var player1AuthResponse = JsonConvert.DeserializeObject<Contracts.Models.Auth.AuthResponse>(player1RegisterBody);

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

        var player2RegisterResponse = await _httpClient.PostAsync("/api/auth/register", player2RegisterContent);
        var player2RegisterBody = await player2RegisterResponse.Content.ReadAsStringAsync();
        
        if (!player2RegisterResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to register player2: {player2RegisterResponse.StatusCode} - {player2RegisterBody}");
        
        var player2AuthResponse = JsonConvert.DeserializeObject<Contracts.Models.Auth.AuthResponse>(player2RegisterBody);

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

        var createTableResponse = await _httpClient.PostAsync("/api/tables", createTableContent);
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

        await _httpClient.PostAsync("/api/tables/join", joinContent1);

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

        await _httpClient.PostAsync("/api/tables/join", joinContent2);

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
        createMatchHttpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", creatorToken);

        var createMatchResponse = await _httpClient.SendAsync(createMatchHttpRequest);
        var createMatchBody = await createMatchResponse.Content.ReadAsStringAsync();
        
        if (!createMatchResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to create match: {createMatchResponse.StatusCode} - {createMatchBody}");
        
        var matchResponse = JsonConvert.DeserializeObject<MatchResponse>(createMatchBody);

        var matchId = matchResponse!.Id;

        // Act - Both players place same bet
        var placeBetRequest = new PlaceBetRequest
        {
            Prediction = "1:1"
        };

        var betContent = new StringContent(
            JsonConvert.SerializeObject(placeBetRequest),
            Encoding.UTF8,
            "application/json");

        var bet1Request = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches/{matchId}/bets")
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(placeBetRequest),
                Encoding.UTF8,
                "application/json")
        };
        bet1Request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", player1Token);

        var bet2Request = new HttpRequestMessage(HttpMethod.Post, $"/api/tables/{tableId}/matches/{matchId}/bets")
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(placeBetRequest),
                Encoding.UTF8,
                "application/json")
        };
        bet2Request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", player2Token);

        var response1 = await _httpClient.SendAsync(bet1Request);
        var response2 = await _httpClient.SendAsync(bet2Request);

        // Assert
        await Verify(new
        {
            Player1StatusCode = response1.StatusCode,
            Player2StatusCode = response2.StatusCode,
            BothSucceeded = response1.IsSuccessStatusCode && response2.IsSuccessStatusCode
        });
    }
}
