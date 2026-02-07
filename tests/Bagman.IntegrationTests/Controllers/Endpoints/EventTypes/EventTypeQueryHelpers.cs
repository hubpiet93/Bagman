namespace Bagman.IntegrationTests.Controllers.Endpoints.EventTypes;

/// <summary>
///     Helper methods for querying event type data in endpoint tests.
/// </summary>
public static class EventTypeQueryHelpers
{
    /// <summary>
    ///     Gets the list of active event types (public endpoint, no authentication required).
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <returns>HttpResponseMessage for snapshot testing.</returns>
    public static async Task<HttpResponseMessage> GetActiveEventTypesAsync(this HttpClient client)
    {
        return await client.GetAsync("/api/event-types");
    }
}
