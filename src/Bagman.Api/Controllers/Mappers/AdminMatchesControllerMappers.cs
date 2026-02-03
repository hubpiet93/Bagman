using Bagman.Application.Features.Matches.CreateMatch;
using Bagman.Contracts.Models;
using Bagman.Contracts.Models.Tables;

namespace Bagman.Api.Controllers.Mappers;

public static class AdminMatchesControllerMappers
{
    public static MatchCreatedResponse ToMatchCreatedResponse(this CreateMatchResult result)
    {
        return new MatchCreatedResponse(
            Id: result.Id,
            EventTypeId: result.EventTypeId,
            Country1: result.Country1,
            Country2: result.Country2,
            MatchDateTime: result.MatchDateTime,
            Status: result.Status,
            CreatedAt: result.CreatedAt
        );
    }
}
