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
        await HttpClient.GetActiveEventTypesAsync();

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateEventType_AsSuperAdmin_ReturnsCreatedWithEventType()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);

        // Act
        await HttpClient.CreateEventTypeAsync(
            "LIGA_MISTRZOW_2026",
            "Liga Mistrzów 2025/2026",
            DateTime.UtcNow.AddMonths(2),
            superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateEventType_WithDuplicateCode_ReturnsConflict()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);
        await HttpClient.CreateEventTypeAsync(
            "DUPLICATE_CODE",
            "First Event",
            DateTime.UtcNow.AddMonths(1),
            superAdminToken);

        // Act - Try to create with same code
        await HttpClient.CreateEventTypeAsync(
            "DUPLICATE_CODE",
            "Second Event",
            DateTime.UtcNow.AddMonths(2),
            superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateEventType_AsRegularUser_ReturnsForbidden()
    {
        // Arrange
        var (regularUserToken, _, _) = await HttpClient.RegisterAndGetTokenAsync("regular_user", TestConstants.DefaultUserPassword);

        // Act
        await HttpClient.CreateEventTypeAsync(
            "FORBIDDEN_EVENT",
            "Forbidden Event",
            DateTime.UtcNow.AddMonths(1),
            regularUserToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task UpdateEventType_AsSuperAdmin_ReturnsOkWithUpdatedEventType()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);

        // Act
        await HttpClient.UpdateEventTypeAsync(
            DefaultEventTypeId,
            "Updated Test Event",
            DateTime.UtcNow.AddMonths(2),
            superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task DeactivateEventType_AsSuperAdmin_ReturnsOkWithDeactivatedEventType()
    {
        // Arrange
        var superAdminToken = await HttpClient.GetSuperAdminTokenAsync(Services);

        // Act
        await HttpClient.DeactivateEventTypeAsync(DefaultEventTypeId, superAdminToken);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task DeactivateEventType_AsRegularUser_ReturnsForbidden()
    {
        // Arrange
        var (regularUserToken, _, _) = await HttpClient.RegisterAndGetTokenAsync("regular_user_deactivate", TestConstants.DefaultUserPassword);

        // Act
        await HttpClient.DeactivateEventTypeAsync(DefaultEventTypeId, regularUserToken);

        // Assert
        await VerifyHttpRecording();
    }
}
