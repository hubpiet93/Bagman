namespace Bagman.IntegrationTests.Controllers.Endpoints.EventTypes;

/// <summary>
///     Helper methods for EventType operations in endpoint tests.
///     Provides admin-only endpoints for creating, updating, and deactivating event types.
/// </summary>
public static class EventTypeOperationHelpers
{
    /// <summary>
    ///     Creates a new event type as a super admin.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="code">Unique code for the event type.</param>
    /// <param name="name">Display name for the event type.</param>
    /// <param name="startDate">Start date for the event type.</param>
    /// <param name="token">The Bearer token of a super admin user.</param>
    /// <returns>HttpResponseMessage from the create endpoint (for snapshot testing).</returns>
    public static async Task<HttpResponseMessage> CreateEventTypeAsync(
        this HttpClient client,
        string code,
        string name,
        DateTime startDate,
        string token)
    {
        var request = new
        {
            Code = code,
            Name = name,
            StartDate = startDate
        };

        return await client.PostAsJsonWithoutDeserializeAsync(
            "/api/admin/event-types",
            request,
            token);
    }

    /// <summary>
    ///     Updates an existing event type as a super admin.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="eventTypeId">The ID of the event type to update.</param>
    /// <param name="name">Updated display name.</param>
    /// <param name="startDate">Updated start date.</param>
    /// <param name="token">The Bearer token of a super admin user.</param>
    /// <returns>HttpResponseMessage from the update endpoint (for snapshot testing).</returns>
    public static async Task<HttpResponseMessage> UpdateEventTypeAsync(
        this HttpClient client,
        Guid eventTypeId,
        string name,
        DateTime startDate,
        string token)
    {
        var request = new
        {
            Name = name,
            StartDate = startDate
        };

        return await client.PutAsJsonWithoutDeserializeAsync(
            $"/api/admin/event-types/{eventTypeId}",
            request,
            token);
    }

    /// <summary>
    ///     Deactivates an event type as a regular user (for testing forbidden access).
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="eventTypeId">The ID of the event type to deactivate.</param>
    /// <param name="token">The Bearer token of a regular (non-admin) user.</param>
    /// <returns>HttpResponseMessage from the deactivate endpoint (for snapshot testing).</returns>
    public static async Task<HttpResponseMessage> DeactivateEventTypeAsync(
        this HttpClient client,
        Guid eventTypeId,
        string token)
    {
        return await client.PostAsJsonWithoutDeserializeAsync(
            $"/api/admin/event-types/{eventTypeId}/deactivate",
            new object(),
            token);
    }
}
