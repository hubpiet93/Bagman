using Bagman.Contracts.Models.Tables;
using Bagman.IntegrationTests.Controllers.Endpoints;
using Bagman.IntegrationTests.Controllers.Endpoints.EventTypes;
using Bagman.IntegrationTests.Controllers.Endpoints.Matches;
using Bagman.IntegrationTests.Controllers.Endpoints.Tables;
using Bagman.IntegrationTests.TestFixtures;
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

    [Fact]
    public async Task GetMatch_WithValidId_ReturnsOkWithMatchResponse()
    {
        // Arrange
        var (tableId, token, _) = await HttpClient.CreateTableAsync(DefaultEventTypeId, "match_get");
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Spain", "Portugal", DateTime.UtcNow.AddDays(5));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Act
        await HttpClient.GetMatchAsync<HttpResponseMessage>(tableId, match.Id, token);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetMatch_StartedFalse_ForFutureDateTime()
    {
        // Arrange
        var (tableId, token, _) = await HttpClient.CreateTableAsync(DefaultEventTypeId, "match_started_future");
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("Poland", "Germany", DateTime.UtcNow.AddHours(2));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Act
        await HttpClient.GetMatchAsync<HttpResponseMessage>(tableId, match.Id, token);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetMatch_StartedTrue_ForPastDateTime()
    {
        // Arrange
        // Create match with date close to now, then wait for it to become "past"
        var (tableId, token, _) = await HttpClient.CreateTableAsync(DefaultEventTypeId, "match_started_past");
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var matchRequest = EventTypeMatchCreation.CreateMatchRequest("France", "Italy", DateTime.UtcNow.AddSeconds(5));
        var match = await HttpClient.CreateMatchAsync<MatchResponse>(DefaultEventTypeId, matchRequest, superAdminToken);

        // Wait for match datetime to become "in the past" relative to current time
        await Task.Delay(6000);

        // Act
        await HttpClient.GetMatchAsync<HttpResponseMessage>(tableId, match.Id, token);

        // Assert
        await VerifyHttpRecording();
    }
}
