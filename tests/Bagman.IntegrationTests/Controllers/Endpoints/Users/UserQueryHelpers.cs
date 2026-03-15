namespace Bagman.IntegrationTests.Controllers.Endpoints.Users;

/// <summary>
///     Helper methods for querying user data in endpoint tests.
///     Each method is generic and can return HttpResponseMessage (for snapshot testing)
///     or a concrete type (for deserialization).
/// </summary>
public static class UserQueryHelpers
{
    /// <summary>
    ///     Gets the profile of the currently authenticated user.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="token">Optional Bearer token for authentication.</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> GetCurrentUserAsync<T>(
        this HttpClient client,
        string? token = null) where T : class
    {
        return client.GetAsync<T>("/api/users/me", token);
    }
}
