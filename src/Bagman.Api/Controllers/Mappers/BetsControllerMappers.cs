using Bagman.Application.Features.Bets.GetUserBet;
using Bagman.Application.Features.Bets.PlaceBet;
using Bagman.Contracts.Models;
using Bagman.Contracts.Models.Tables;

namespace Bagman.Api.Controllers.Mappers;

public static class BetsControllerMappers
{
    public static BetResponse ToBetResponse(this PlaceBetResult result)
    {
        return new BetResponse
        {
            Id = result.Id,
            UserId = result.UserId,
            MatchId = result.MatchId,
            Prediction = result.Prediction,
            EditedAt = result.EditedAt
        };
    }

    public static BetResponse ToBetResponse(this UserBetResult result)
    {
        return new BetResponse
        {
            Id = result.Id,
            UserId = result.UserId,
            MatchId = result.MatchId,
            Prediction = result.Prediction,
            EditedAt = result.EditedAt
        };
    }

    public static UserBetsResponse ToUserBetsResponse(this List<UserBetResult> results)
    {
        return new UserBetsResponse(
            results.Select(r => r.ToBetResponse()).ToList()
        );
    }
}
