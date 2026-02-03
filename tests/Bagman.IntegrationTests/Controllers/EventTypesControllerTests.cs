using System.Net.Http.Headers;
using System.Text;
using Bagman.Contracts.Models.Auth;
using Bagman.IntegrationTests.TestFixtures;
using Newtonsoft.Json;
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
        // Act - Get active EventTypes (public endpoint, no auth)
        await HttpClient!.GetAsync("/api/event-types");

        // Assert - Should return active EventTypes including the default one
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateEventType_AsSuperAdmin_ReturnsCreatedWithEventType()
    {
        // Arrange
        var superAdminToken = await GetSuperAdminToken();

        var createRequest = new
        {
            Code = "LIGA_MISTRZOW_2026",
            Name = "Liga Mistrzów 2025/2026",
            StartDate = DateTime.UtcNow.AddMonths(2)
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(createRequest),
            Encoding.UTF8,
            "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admin/event-types")
        {
            Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", superAdminToken);

        // Act
        await HttpClient!.SendAsync(request);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateEventType_WithDuplicateCode_ReturnsConflict()
    {
        // Arrange
        var superAdminToken = await GetSuperAdminToken();

        var createFirstRequest = new
        {
            Code = "DUPLICATE_CODE",
            Name = "First Event",
            StartDate = DateTime.UtcNow.AddMonths(1)
        };

        var firstContent = new StringContent(
            JsonConvert.SerializeObject(createFirstRequest),
            Encoding.UTF8,
            "application/json");

        var firstRequest = new HttpRequestMessage(HttpMethod.Post, "/api/admin/event-types")
        {
            Content = firstContent
        };
        firstRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", superAdminToken);

        await HttpClient!.SendAsync(firstRequest);

        // Act - Try to create with same code
        var createSecondRequest = new
        {
            Code = "DUPLICATE_CODE",
            Name = "Second Event",
            StartDate = DateTime.UtcNow.AddMonths(2)
        };

        var secondContent = new StringContent(
            JsonConvert.SerializeObject(createSecondRequest),
            Encoding.UTF8,
            "application/json");

        var secondRequest = new HttpRequestMessage(HttpMethod.Post, "/api/admin/event-types")
        {
            Content = secondContent
        };
        secondRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", superAdminToken);

        await HttpClient.SendAsync(secondRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task CreateEventType_AsRegularUser_ReturnsForbidden()
    {
        // Arrange - Register regular user (not SuperAdmin)
        var registerRequest = new RegisterRequest
        {
            Login = "regular_user",
            Password = "Regular@12345",
            Email = "regular@test.com"
        };

        var registerContent = new StringContent(
            JsonConvert.SerializeObject(registerRequest),
            Encoding.UTF8,
            "application/json");

        var registerResponse = await HttpClient!.PostAsync("/api/auth/register", registerContent);
        var registerBody = await registerResponse.Content.ReadAsStringAsync();
        var authResponse = JsonConvert.DeserializeObject<AuthResponse>(registerBody);
        var regularUserToken = authResponse!.AccessToken;

        // Act - Try to create EventType as regular user
        var createRequest = new
        {
            Code = "FORBIDDEN_EVENT",
            Name = "Forbidden Event",
            StartDate = DateTime.UtcNow.AddMonths(1)
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(createRequest),
            Encoding.UTF8,
            "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admin/event-types")
        {
            Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", regularUserToken);

        await HttpClient.SendAsync(request);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task UpdateEventType_AsSuperAdmin_ReturnsOkWithUpdatedEventType()
    {
        // Arrange - Use default EventType
        var superAdminToken = await GetSuperAdminToken();

        // Act - Update default EventType
        var updateRequest = new
        {
            Name = "Updated Test Event",
            StartDate = DateTime.UtcNow.AddMonths(2)
        };

        var updateContent = new StringContent(
            JsonConvert.SerializeObject(updateRequest),
            Encoding.UTF8,
            "application/json");

        var updateHttpRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/admin/event-types/{DefaultEventTypeId}")
        {
            Content = updateContent
        };
        updateHttpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", superAdminToken);

        await HttpClient!.SendAsync(updateHttpRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task DeactivateEventType_AsSuperAdmin_ReturnsOkWithDeactivatedEventType()
    {
        // Arrange - Use default EventType created in InitializeDatabase
        var superAdminToken = await GetSuperAdminToken();

        // Act - Deactivate default EventType
        var deactivateRequest = new HttpRequestMessage(
            HttpMethod.Post, 
            $"/api/admin/event-types/{DefaultEventTypeId}/deactivate");
        deactivateRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", superAdminToken);

        await HttpClient!.SendAsync(deactivateRequest);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task DeactivateEventType_AsRegularUser_ReturnsForbidden()
    {
        // Arrange - Register regular user
        var registerRequest = new RegisterRequest
        {
            Login = "regular_user_deactivate",
            Password = "Regular@12345",
            Email = "regular_deactivate@test.com"
        };

        var registerContent = new StringContent(
            JsonConvert.SerializeObject(registerRequest),
            Encoding.UTF8,
            "application/json");

        var registerResponse = await HttpClient!.PostAsync("/api/auth/register", registerContent);
        var registerBody = await registerResponse.Content.ReadAsStringAsync();
        var authResponse = JsonConvert.DeserializeObject<AuthResponse>(registerBody);
        var regularUserToken = authResponse!.AccessToken;

        // Act - Try to deactivate default EventType as regular user
        var deactivateRequest = new HttpRequestMessage(
            HttpMethod.Post, 
            $"/api/admin/event-types/{DefaultEventTypeId}/deactivate");
        deactivateRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", regularUserToken);

        await HttpClient.SendAsync(deactivateRequest);

        // Assert
        await VerifyHttpRecording();
    }
}
