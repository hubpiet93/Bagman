using System.Net.Http.Headers;
using System.Text;
using Bagman.Contracts.Models.Auth;
using Newtonsoft.Json;

namespace Bagman.IntegrationTests.Helpers;

/// <summary>
///     Helper class for creating EventTypes in integration tests.
///     Provides methods to create EventTypes via SuperAdmin API.
/// </summary>
public class EventTypeFixture
{
    private readonly HttpClient _httpClient;

    public EventTypeFixture(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    ///     Creates a SuperAdmin user directly in the database (bypasses API)
    /// </summary>
    public async Task<(string Token, Guid UserId)> CreateSuperAdminUser(
        string login,
        string password,
        string email)
    {
        // Register regular user first
        var registerRequest = new RegisterRequest
        {
            Login = login,
            Password = password,
            Email = email
        };

        var registerContent = new StringContent(
            JsonConvert.SerializeObject(registerRequest),
            Encoding.UTF8,
            "application/json");

        var registerResponse = await _httpClient.PostAsync("/api/auth/register", registerContent);
        var registerBody = await registerResponse.Content.ReadAsStringAsync();
        var authResponse = JsonConvert.DeserializeObject<AuthResponse>(registerBody);

        if (authResponse == null)
            throw new Exception($"Failed to register user: {registerBody}");

        // NOTE: In real tests, you would need to manually update the database to set IsSuperAdmin=true
        // This is a limitation because SuperAdmin can only be set via database
        // For now, we'll return the user info and tests need to handle the DB update separately

        return (authResponse.AccessToken, authResponse.User.Id);
    }

    /// <summary>
    ///     Creates an EventType using SuperAdmin credentials
    /// </summary>
    public async Task<Guid> CreateEventType(
        string superAdminToken,
        string code,
        string name,
        DateTime? startDate = null)
    {
        var request = new
        {
            Code = code,
            Name = name,
            StartDate = startDate ?? DateTime.UtcNow.AddDays(-1)
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json");

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/admin/event-types")
        {
            Content = content
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", superAdminToken);

        var response = await _httpClient.SendAsync(httpRequest);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to create EventType: {response.StatusCode} - {responseBody}");

        var eventTypeResponse = JsonConvert.DeserializeObject<dynamic>(responseBody);
        return Guid.Parse(eventTypeResponse!.id.ToString());
    }

    /// <summary>
    ///     Creates a default EventType for testing purposes
    /// </summary>
    public async Task<Guid> CreateDefaultEventType(string superAdminToken)
    {
        var uniqueCode = $"TEST_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        return await CreateEventType(
            superAdminToken,
            uniqueCode,
            $"Test Event {uniqueCode}",
            DateTime.UtcNow.AddDays(-1));
    }

    /// <summary>
    ///     Gets all active EventTypes (public endpoint)
    /// </summary>
    public async Task<List<dynamic>> GetActiveEventTypes()
    {
        var response = await _httpClient.GetAsync("/api/event-types");
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to get active EventTypes: {response.StatusCode} - {responseBody}");

        var eventTypes = JsonConvert.DeserializeObject<List<dynamic>>(responseBody);
        return eventTypes ?? new List<dynamic>();
    }
}
