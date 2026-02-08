using Bagman.Application.Features.Matches.CreateMatch;
using Bagman.Contracts.Models;

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
}
