using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Bagman.IntegrationTests.Controllers.Endpoints;

/// <summary>
///     Extension methods for HttpClient to simplify JSON serialization and Bearer token handling in tests.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    ///     Sends a POST request with JSON content and automatically deserializes the response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response content into.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="request">The request object to serialize as JSON.</param>
    /// <param name="token">Optional Bearer token for authentication.</param>
    /// <returns>The deserialized response of type T.</returns>
    public static async Task<T> PostAsJsonAsync<T>(
        this HttpClient client,
        string endpoint,
        object request,
        string? token = null)
    {
        var content = new StringContent(
            JsonConvert.SerializeObject(request),
            System.Text.Encoding.UTF8,
            "application/json");

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = content };

        if (token != null)
        {
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await client.SendAsync(httpRequest);
        var body = await response.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<T>(body)!;
    }

    /// <summary>
    ///     Sends a GET request with Bearer token authentication and automatically deserializes the response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response content into.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>The deserialized response of type T.</returns>
    public static async Task<T> GetAsJsonAsync<T>(this HttpClient client, string endpoint, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<T>(body)!;
    }

    /// <summary>
    ///     Sends a DELETE request with Bearer token authentication and automatically deserializes the response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response content into.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>The deserialized response of type T.</returns>
    public static async Task<T> DeleteAsJsonAsync<T>(this HttpClient client, string endpoint, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<T>(body)!;
    }

    /// <summary>
    ///     Sends a PUT request with JSON content and Bearer token authentication, deserializes the response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response content into.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="request">The request object to serialize as JSON.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>The deserialized response of type T.</returns>
    public static async Task<T> PutAsJsonAsync<T>(
        this HttpClient client,
        string endpoint,
        object request,
        string token)
    {
        var content = new StringContent(
            JsonConvert.SerializeObject(request),
            System.Text.Encoding.UTF8,
            "application/json");

        var httpRequest = new HttpRequestMessage(HttpMethod.Put, endpoint) { Content = content };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(httpRequest);
        var body = await response.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<T>(body)!;
    }

    /// <summary>
    ///     Sends a POST request without authentication that doesn't deserialize the response.
    ///     Used for snapshot testing where we just need the raw HTTP call recorded.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="request">The request object to serialize as JSON.</param>
    /// <returns>The HttpResponseMessage.</returns>
    public static async Task<HttpResponseMessage> PostAsJsonWithoutDeserializeAsync(
        this HttpClient client,
        string endpoint,
        object request)
    {
        var content = new StringContent(
            JsonConvert.SerializeObject(request),
            System.Text.Encoding.UTF8,
            "application/json");

        return await client.PostAsync(endpoint, content);
    }

    /// <summary>
    ///     Sends a POST request with Bearer token authentication that doesn't deserialize the response.
    ///     Used for snapshot testing where we just need the raw HTTP call recorded.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="request">The request object to serialize as JSON.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>The HttpResponseMessage.</returns>
    public static async Task<HttpResponseMessage> PostAsJsonWithoutDeserializeAsync(
        this HttpClient client,
        string endpoint,
        object request,
        string token)
    {
        var content = new StringContent(
            JsonConvert.SerializeObject(request),
            System.Text.Encoding.UTF8,
            "application/json");

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = content };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await client.SendAsync(httpRequest);
    }

    /// <summary>
    ///     Sends a PUT request with Bearer token authentication that doesn't deserialize the response.
    ///     Used for snapshot testing where we just need the raw HTTP call recorded.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="request">The request object to serialize as JSON.</param>
    /// <param name="token">The Bearer token for authentication.</param>
    /// <returns>The HttpResponseMessage.</returns>
    public static async Task<HttpResponseMessage> PutAsJsonWithoutDeserializeAsync(
        this HttpClient client,
        string endpoint,
        object request,
        string token)
    {
        var content = new StringContent(
            JsonConvert.SerializeObject(request),
            System.Text.Encoding.UTF8,
            "application/json");

        var httpRequest = new HttpRequestMessage(HttpMethod.Put, endpoint) { Content = content };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await client.SendAsync(httpRequest);
    }
}
