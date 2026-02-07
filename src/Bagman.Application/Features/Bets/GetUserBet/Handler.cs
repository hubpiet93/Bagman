using Bagman.Application.Common;
using Bagman.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Application.Features.Bets.GetUserBet;

public record GetUserBetQuery
{
    public required Guid MatchId { get; init; }
    public required Guid UserId { get; init; }
}

public record UserBetResult
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required Guid MatchId { get; init; }
    public required string Prediction { get; init; }
    public required DateTime EditedAt { get; init; }
}

public class GetUserBetHandler : IFeatureHandler<GetUserBetQuery, UserBetResult>
{
    private readonly ApplicationDbContext _dbContext;

    public GetUserBetHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<UserBetResult>> HandleAsync(
        GetUserBetQuery request,
        CancellationToken cancellationToken = default)
    {
        var match = await _dbContext.Matches
            .Where(m => m.Id == request.MatchId)
            .FirstOrDefaultAsync(cancellationToken);

        if (match == null)
            return Error.NotFound("Match.NotFound", "Mecz nie został znaleziony");

        var bet = match.GetUserBet(request.UserId);
        
        if (bet == null)
            return Error.NotFound("Bet.NotFound", "Typ nie został znaleziony");

        return new UserBetResult
        {
            Id = bet.Id,
            UserId = bet.UserId,
            MatchId = bet.MatchId,
            Prediction = bet.Prediction.Value,
            EditedAt = bet.EditedAt
        };
    }
}
