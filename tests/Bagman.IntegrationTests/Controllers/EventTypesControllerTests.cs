using Bagman.IntegrationTests.Controllers.Endpoints;
using Bagman.IntegrationTests.Controllers.Endpoints.EventTypes;
using Bagman.IntegrationTests.Helpers;
using Bagman.IntegrationTests.TestFixtures;
using Xunit.Abstractions;

namespace Bagman.IntegrationTests.Controllers;

[CollectionDefinition("EventTypes Tests")]
public class EventTypesTestsCollection : ICollectionFixture<PostgresFixture>
{
}

/// <summary>
///     Integration tests for EventTypesController actions using TestContainers for PostgreSQL.
///     Tests public endpoints (GetActiveEventTypes) and SuperAdmin endpoints (Create, Update, Deactivate).
/// </summary>
[Collection("EventTypes Tests")]
public class EventTypesControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    public EventTypesControllerTests(PostgresFixture postgresFixture, ITestOutputHelper testOutputHelper)
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

    [Fact]
    public async Task GetActiveEventTypes_WithoutAuthentication_ReturnsOkWithActiveEventTypes()
    {
        // Arrange - Default EventType is already created in InitializeDatabase

        // Act
        await HttpClient.GetActiveEventTypesAsync<HttpResponseMessage>();

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateEventType_AsSuperAdmin_ReturnsCreatedWithEventType()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var request = EventTypeOperationHelpers.CreateEventTypeRequest(
            "LIGA_MISTRZOW_2026",
            "Liga Mistrzów 2025/2026",
            DateTime.UtcNow.AddMonths(2));

        // Act
        await HttpClient.CreateEventTypeAsync<HttpResponseMessage>(request, superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateEventType_WithDuplicateCode_ReturnsConflict()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var request1 = EventTypeOperationHelpers.CreateEventTypeRequest(
            "DUPLICATE_CODE",
            "First Event",
            DateTime.UtcNow.AddMonths(1));
        await HttpClient.CreateEventTypeAsync<HttpResponseMessage>(request1, superAdminToken);

        // Act - Try to create with same code
        var request2 = EventTypeOperationHelpers.CreateEventTypeRequest(
            "DUPLICATE_CODE",
            "Second Event",
            DateTime.UtcNow.AddMonths(2));
        await HttpClient.CreateEventTypeAsync<HttpResponseMessage>(request2, superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateEventType_AsRegularUser_ReturnsForbidden()
    {
        // Arrange
        var (regularUserToken, _, _) = await HttpClient.RegisterAndGetTokenAsync("regular_user", TestConstants.DefaultUserPassword);
        var request = EventTypeOperationHelpers.CreateEventTypeRequest(
            "FORBIDDEN_EVENT",
            "Forbidden Event",
            DateTime.UtcNow.AddMonths(1));

        // Act
        await HttpClient.CreateEventTypeAsync<HttpResponseMessage>(request, regularUserToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task UpdateEventType_AsSuperAdmin_ReturnsOkWithUpdatedEventType()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        var request = EventTypeOperationHelpers.CreateUpdateEventTypeRequest(
            "Updated Test Event",
            DateTime.UtcNow.AddMonths(2));

        // Act
        await HttpClient.UpdateEventTypeAsync<HttpResponseMessage>(DefaultEventTypeId, request, superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task DeactivateEventType_AsSuperAdmin_ReturnsOkWithDeactivatedEventType()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);

        // Act
        await HttpClient.DeactivateEventTypeAsync<HttpResponseMessage>(DefaultEventTypeId, superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task DeactivateEventType_AsRegularUser_ReturnsForbidden()
    {
        // Arrange
        var (regularUserToken, _, _) = await HttpClient.RegisterAndGetTokenAsync("regular_user_deactivate", TestConstants.DefaultUserPassword);

        // Act
        await HttpClient.DeactivateEventTypeAsync<HttpResponseMessage>(DefaultEventTypeId, regularUserToken);

        // Assert
        await VerifyHttpRecording();
    }
}
