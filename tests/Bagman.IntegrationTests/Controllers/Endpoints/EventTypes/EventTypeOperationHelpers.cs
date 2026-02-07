namespace Bagman.IntegrationTests.Controllers.Endpoints.EventTypes;

/// <summary>
///     Helper methods for EventType operations in endpoint tests.
///     Provides admin-only endpoints for creating, updating, and deactivating event types.
///     Each method is generic and can return HttpResponseMessage (for snapshot testing)
///     or a concrete type (for deserialization).
/// </summary>
public static class EventTypeOperationHelpers
{
    /// <summary>
    ///     Creates a new event type as a super admin.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="request">The create event type request.</param>
    /// <param name="token">Optional Bearer token of a super admin user.</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> CreateEventTypeAsync<T>(
        this HttpClient client,
        object request,
        string? token = null) where T : class
    {
        return client.PostAsync<T>("/api/admin/event-types", request, token);
    }

    /// <summary>
    ///     Updates an existing event type as a super admin.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="eventTypeId">The ID of the event type to update.</param>
    /// <param name="request">The update event type request.</param>
    /// <param name="token">Optional Bearer token of a super admin user.</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> UpdateEventTypeAsync<T>(
        this HttpClient client,
        Guid eventTypeId,
        object request,
        string? token = null) where T : class
    {
        return client.PutAsync<T>($"/api/admin/event-types/{eventTypeId}", request, token);
    }

    /// <summary>
    ///     Deactivates an event type.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="eventTypeId">The ID of the event type to deactivate.</param>
    /// <param name="token">Optional Bearer token (super admin for success, regular user for testing forbidden).</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> DeactivateEventTypeAsync<T>(
        this HttpClient client,
        Guid eventTypeId,
        string? token = null) where T : class
    {
        return client.PostAsync<T>($"/api/admin/event-types/{eventTypeId}/deactivate", new object(), token);
    }

    #region Request Factory Methods

    /// <summary>
    ///     Creates a request object for creating an event type.
    /// </summary>
    public static object CreateEventTypeRequest(string code, string name, DateTime startDate)
    {
        return new { Code = code, Name = name, StartDate = startDate };
    }

    /// <summary>
    ///     Creates a request object for updating an event type.
    /// </summary>
    public static object CreateUpdateEventTypeRequest(string name, DateTime startDate)
    {
        return new { Name = name, StartDate = startDate };
    }

    #endregion
}
