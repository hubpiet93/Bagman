namespace Bagman.IntegrationTests.Controllers.Endpoints.EventTypes;

/// <summary>
///     Helper methods for querying event type data in endpoint tests.
///     Each method is generic and can return HttpResponseMessage (for snapshot testing)
///     or a concrete type (for deserialization).
/// </summary>
public static class EventTypeQueryHelpers
{
    /// <summary>
    ///     Gets the list of active event types (public endpoint, no authentication required).
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="token">Optional Bearer token (typically null for this public endpoint).</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> GetActiveEventTypesAsync<T>(
        this HttpClient client,
        string? token = null) where T : class
    {
        return client.GetAsync<T>("/api/event-types", token);
    }
}
