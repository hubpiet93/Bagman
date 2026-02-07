using Bagman.Contracts.Models.Tables;

namespace Bagman.IntegrationTests.Controllers.Endpoints.Matches;

/// <summary>
///     Helper methods for setting match results in endpoint tests.
///     Each method is generic and can return HttpResponseMessage (for snapshot testing)
///     or a concrete type (for deserialization).
/// </summary>
public static class MatchResultHelpers
{
    /// <summary>
    ///     Sets the result for a match (SuperAdmin only).
    /// </summary>
    /// <typeparam name="T">HttpResponseMessage for snapshot testing, or a concrete type for deserialization.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="matchId">The ID of the match.</param>
    /// <param name="request">The set match result request.</param>
    /// <param name="token">Bearer token of a super admin user.</param>
    /// <returns>Response of type T.</returns>
    public static Task<T> SetMatchResultAsync<T>(
        this HttpClient client,
        Guid matchId,
        SetMatchResultRequest request,
        string? token = null) where T : class
    {
        return client.PostAsync<T>($"/api/admin/matches/{matchId}/result", request, token);
    }

    /// <summary>
    ///     Creates a SetMatchResultRequest with the specified result.
    /// </summary>
    /// <param name="result">The match result (e.g., "2:1", "0:0").</param>
    /// <returns>A SetMatchResultRequest.</returns>
    public static SetMatchResultRequest CreateSetResultRequest(string result)
    {
        return new SetMatchResultRequest { Result = result };
    }
}
