using Bagman.Contracts.Models.Tables;
using Bagman.IntegrationTests.Controllers.Endpoints;
using Bagman.IntegrationTests.Controllers.Endpoints.EventTypes;
using Bagman.IntegrationTests.Controllers.Endpoints.Matches;
using Bagman.IntegrationTests.Helpers;
using Bagman.IntegrationTests.TestFixtures;
using Xunit.Abstractions;

namespace Bagman.IntegrationTests.Controllers;

[CollectionDefinition("AdminMatches Tests")]
public class AdminMatchesTestsCollection : ICollectionFixture<PostgresFixture>
{
}

/// <summary>
///     Integration tests for AdminMatchesController actions using TestContainers for PostgreSQL.
///     Tests SuperAdmin-only endpoints: GetMatchesByEventType, CreateMatch, UpdateMatch, DeleteMatch, SetMatchResult.
/// </summary>
[Collection("AdminMatches Tests")]
public class AdminMatchesControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    public AdminMatchesControllerTests(PostgresFixture postgresFixture, ITestOutputHelper testOutputHelper)
        : base(postgresFixture, testOutputHelper)
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

    // ── GetMatchesByEventType ────────────────────────────────────────────────

    [Fact]
    public async Task GetMatchesByEventType_AsSuperAdmin_ReturnsOkWithMatchList()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Spain", "Portugal", DateTime.UtcNow.AddDays(5));
        await HttpClient.CreateMatchAsync<HttpResponseMessage>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Act
        await HttpClient.GetMatchesByEventTypeAsync<HttpResponseMessage>(DefaultEventTypeId, superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetMatchesByEventType_WithFinishedMatch_ReturnsResultField()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        await MatchDataSeedingHelpers.SeedFinishedMatchAsync(
            Services, DefaultEventTypeId, "Brazil", "Argentina", "3:0");

        // Act
        await HttpClient.GetMatchesByEventTypeAsync<HttpResponseMessage>(DefaultEventTypeId, superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetMatchesByEventType_AsRegularUser_ReturnsForbidden()
    {
        // Arrange
        var (regularUserToken, _, _) = await HttpClient.RegisterAndGetTokenAsync("regular_matches", TestConstants.DefaultUserPassword);

        // Act
        await HttpClient.GetMatchesByEventTypeAsync<HttpResponseMessage>(DefaultEventTypeId, regularUserToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetMatchesByEventType_WithNonExistentEventType_ReturnsNotFound()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var nonExistentId = Guid.NewGuid();

        // Act
        await HttpClient.GetMatchesByEventTypeAsync<HttpResponseMessage>(nonExistentId, superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    // ── CreateMatch ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateMatch_AsSuperAdmin_ReturnsCreatedWithMatch()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("France", "Germany", DateTime.UtcNow.AddDays(7));

        // Act
        await HttpClient.CreateMatchAsync<HttpResponseMessage>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateMatch_AsRegularUser_ReturnsForbidden()
    {
        // Arrange
        var (regularUserToken, _, _) = await HttpClient.RegisterAndGetTokenAsync("regular_create_match", TestConstants.DefaultUserPassword);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Italy", "England", DateTime.UtcNow.AddDays(3));

        // Act
        await HttpClient.CreateMatchAsync<HttpResponseMessage>(DefaultEventTypeId, matchRequest, regularUserToken);

        // Assert
        await VerifyHttpRecording();
    }

    // ── UpdateMatch ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateMatch_AsSuperAdmin_ReturnsOkWithUpdatedMatch()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Poland", "Croatia", DateTime.UtcNow.AddDays(4));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        var updateRequest = new
        {
            Country1 = "Poland",
            Country2 = "Czech Republic",
            MatchDateTime = DateTime.UtcNow.AddDays(10)
        };

        // Act
        await HttpClient.PutAsync<HttpResponseMessage>(
            $"/api/admin/event-types/{DefaultEventTypeId}/matches/{match.Id}",
            updateRequest,
            superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task UpdateMatch_WithStartedMatch_ReturnsBadRequest()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchId = await MatchDataSeedingHelpers.SeedMatchAsync(
            Services, DefaultEventTypeId, "Turkey", "Netherlands",
            DateTime.UtcNow.AddMinutes(-5), "scheduled");

        var updateRequest = new
        {
            Country1 = "Turkey",
            Country2 = "Belgium",
            MatchDateTime = DateTime.UtcNow.AddDays(2)
        };

        // Act
        await HttpClient.PutAsync<HttpResponseMessage>(
            $"/api/admin/event-types/{DefaultEventTypeId}/matches/{matchId}",
            updateRequest,
            superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    // ── DeleteMatch ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteMatch_AsSuperAdmin_ReturnsOk()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Sweden", "Denmark", DateTime.UtcNow.AddDays(6));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Act
        await HttpClient.DeleteAsync<HttpResponseMessage>(
            $"/api/admin/event-types/{DefaultEventTypeId}/matches/{match.Id}",
            superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task DeleteMatch_WithStartedMatch_ReturnsBadRequest()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchId = await MatchDataSeedingHelpers.SeedMatchAsync(
            Services, DefaultEventTypeId, "Serbia", "Switzerland",
            DateTime.UtcNow.AddMinutes(-10), "scheduled");

        // Act
        await HttpClient.DeleteAsync<HttpResponseMessage>(
            $"/api/admin/event-types/{DefaultEventTypeId}/matches/{matchId}",
            superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    // ── SetMatchResult ───────────────────────────────────────────────────────

    [Fact]
    public async Task SetMatchResult_AsSuperAdmin_ReturnsOkWithFinishedMatch()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchId = await MatchDataSeedingHelpers.SeedMatchAsync(
            Services, DefaultEventTypeId, "Spain", "Italy",
            DateTime.UtcNow.AddMinutes(-30), "scheduled");

        var resultRequest = MatchResultHelpers.CreateSetResultRequest("2:1");

        // Act
        await HttpClient.SetMatchResultAsync<HttpResponseMessage>(matchId, resultRequest, superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task SetMatchResult_WithInvalidFormat_ReturnsBadRequest()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchId = await MatchDataSeedingHelpers.SeedMatchAsync(
            Services, DefaultEventTypeId, "Portugal", "Morocco",
            DateTime.UtcNow.AddMinutes(-15), "scheduled");

        var resultRequest = MatchResultHelpers.CreateSetResultRequest("invalid-format");

        // Act
        await HttpClient.SetMatchResultAsync<HttpResponseMessage>(matchId, resultRequest, superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task SetMatchResult_WithFutureMatch_ReturnsBadRequest()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("England", "Belgium", DateTime.UtcNow.AddDays(2));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        var resultRequest = MatchResultHelpers.CreateSetResultRequest("1:0");

        // Act
        await HttpClient.SetMatchResultAsync<HttpResponseMessage>(match.Id, resultRequest, superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task SetMatchResult_AsRegularUser_ReturnsForbidden()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var (regularUserToken, _, _) = await HttpClient.RegisterAndGetTokenAsync("regular_set_result", TestConstants.DefaultUserPassword);
        var matchId = await MatchDataSeedingHelpers.SeedMatchAsync(
            Services, DefaultEventTypeId, "Japan", "Australia",
            DateTime.UtcNow.AddMinutes(-5), "scheduled");

        var resultRequest = MatchResultHelpers.CreateSetResultRequest("1:1");

        // Act
        await HttpClient.SetMatchResultAsync<HttpResponseMessage>(matchId, resultRequest, regularUserToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task SetMatchResult_WithNonExistentMatch_ReturnsNotFound()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var nonExistentId = Guid.NewGuid();
        var resultRequest = MatchResultHelpers.CreateSetResultRequest("0:0");

        // Act
        await HttpClient.SetMatchResultAsync<HttpResponseMessage>(nonExistentId, resultRequest, superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }
}
