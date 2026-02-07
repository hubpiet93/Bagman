using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace Bagman.IntegrationTests.Controllers.Endpoints;

/// <summary>
///     Extension methods for HttpClient to simplify JSON serialization and Bearer token handling in tests.
///     All methods are generic and can return either HttpResponseMessage (for snapshot testing)
///     or a deserialized type.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    ///     Sends a POST request with JSON content.
    ///     Returns HttpResponseMessage for snapshot testing or deserializes to T.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for raw response, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="request">The request object to serialize as JSON.</param>
    /// <param name="token">Optional Bearer token for authentication. When null, no Authorization header is added.</param>
    /// <returns>Response of type T.</returns>
    public static async Task<T> PostAsync<T>(
        this HttpClient client,
        string endpoint,
        object request,
        string? token = null) where T : class
    {
        var content = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json");

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = content };

        if (token != null)
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(httpRequest);

        if (typeof(T) == typeof(HttpResponseMessage))
            return (response as T)!;

        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(body)!;
    }

    /// <summary>
    ///     Sends a GET request.
    ///     Returns HttpResponseMessage for snapshot testing or deserializes to T.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for raw response, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="token">Optional Bearer token for authentication.</param>
    /// <returns>Response of type T.</returns>
    public static async Task<T> GetAsync<T>(
        this HttpClient client,
        string endpoint,
        string? token = null) where T : class
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);

        if (token != null)
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(httpRequest);

        if (typeof(T) == typeof(HttpResponseMessage))
            return (response as T)!;

        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(body)!;
    }

    /// <summary>
    ///     Sends a PUT request with JSON content.
    ///     Returns HttpResponseMessage for snapshot testing or deserializes to T.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for raw response, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="request">The request object to serialize as JSON.</param>
    /// <param name="token">Optional Bearer token for authentication.</param>
    /// <returns>Response of type T.</returns>
    public static async Task<T> PutAsync<T>(
        this HttpClient client,
        string endpoint,
        object request,
        string? token = null) where T : class
    {
        var content = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json");

        var httpRequest = new HttpRequestMessage(HttpMethod.Put, endpoint) { Content = content };

        if (token != null)
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(httpRequest);

        if (typeof(T) == typeof(HttpResponseMessage))
            return (response as T)!;

        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(body)!;
    }

    /// <summary>
    ///     Sends a DELETE request without body.
    ///     Returns HttpResponseMessage for snapshot testing or deserializes to T.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for raw response, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="token">Optional Bearer token for authentication.</param>
    /// <returns>Response of type T.</returns>
    public static async Task<T> DeleteAsync<T>(
        this HttpClient client,
        string endpoint,
        string? token = null) where T : class
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, endpoint);

        if (token != null)
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(httpRequest);

        if (typeof(T) == typeof(HttpResponseMessage))
            return (response as T)!;

        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(body)!;
    }

    /// <summary>
    ///     Sends a DELETE request with JSON body.
    ///     Returns HttpResponseMessage for snapshot testing or deserializes to T.
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for raw response, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="request">The request object to serialize as JSON.</param>
    /// <param name="token">Optional Bearer token for authentication.</param>
    /// <returns>Response of type T.</returns>
    public static async Task<T> DeleteAsync<T>(
        this HttpClient client,
        string endpoint,
        object request,
        string? token = null) where T : class
    {
        var content = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json");

        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, endpoint) { Content = content };

        if (token != null)
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(httpRequest);

        if (typeof(T) == typeof(HttpResponseMessage))
            return (response as T)!;

        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(body)!;
    }
}
