using Bagman.Application.Features.Matches.CreateMatch;
using Bagman.Application.Features.Matches.GetMatchesByEventType;
using Bagman.Application.Features.Matches.UpdateMatch;
using Bagman.Contracts.Models;
using Bagman.Contracts.Models.Tables;

namespace Bagman.Api.Controllers.Mappers;

public static class AdminMatchesControllerMappers
{
    public static MatchCreatedResponse ToMatchCreatedResponse(this CreateMatchResult result)
    {
        return new MatchCreatedResponse(
            result.Id,
            result.EventTypeId,
            result.Country1,
            result.Country2,
            result.MatchDateTime,
            result.Status,
            result.CreatedAt
        );
    }

    public static MatchResponse ToMatchResponse(this MatchListItemResult result)
    {
        return new MatchResponse
        {
            Id = result.Id,
            EventTypeId = result.EventTypeId,
            Country1 = result.Country1,
            Country2 = result.Country2,
            MatchDateTime = result.MatchDateTime,
            Result = null,
            Status = result.Status,
            Started = DateTime.UtcNow >= result.MatchDateTime,
            CreatedAt = result.CreatedAt
        };
    }

    public static MatchResponse ToMatchResponse(this UpdateMatchResult result)
    {
        return new MatchResponse
        {
            Id = result.Id,
            EventTypeId = result.EventTypeId,
            Country1 = result.Country1,
            Country2 = result.Country2,
            MatchDateTime = result.MatchDateTime,
            Result = null,
            Status = result.Status,
            Started = DateTime.UtcNow >= result.MatchDateTime,
            CreatedAt = result.CreatedAt
        };
    }
}
