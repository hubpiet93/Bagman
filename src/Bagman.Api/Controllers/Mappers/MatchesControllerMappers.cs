using Bagman.Application.Features.Matches.GetMatchDetails;
using Bagman.Contracts.Models.Tables;

namespace Bagman.Api.Controllers.Mappers;

public static class MatchesControllerMappers
{
    public static MatchResponse ToMatchResponse(this MatchDetailsResult result)
    {
        return new MatchResponse
        {
            Id = result.Id,
            EventTypeId = result.EventTypeId,
            Country1 = result.Country1,
            Country2 = result.Country2,
            MatchDateTime = result.MatchDateTime,
            Result = result.Result,
            Status = result.Status,
            Started = DateTime.UtcNow >= result.MatchDateTime,
            CreatedAt = result.CreatedAt
        };
    }
}
