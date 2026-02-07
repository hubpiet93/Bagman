using Bagman.Application.Common;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.Bets.DeleteBet;

public record DeleteBetCommand
{
    public required Guid MatchId { get; init; }
    public required Guid UserId { get; init; }
}

public class DeleteBetHandler : IFeatureHandler<DeleteBetCommand, Success>
{
    private readonly IMatchRepository _matchRepository;

    public DeleteBetHandler(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public async Task<ErrorOr<Success>> HandleAsync(
        DeleteBetCommand request,
        CancellationToken cancellationToken = default)
    {
        // Get match aggregate
        var matchResult = await _matchRepository.GetByIdAsync(request.MatchId);
        if (matchResult.IsError)
            return matchResult.Errors;

        if (matchResult.Value == null)
            return Error.NotFound("Match.NotFound", "Mecz nie został znaleziony");

        var match = matchResult.Value;

        // Remove bet through aggregate
        var removeBetResult = match.RemoveBet(request.UserId);
        if (removeBetResult.IsError)
            return removeBetResult.Errors;

        // Persist changes (EF change tracking automatically detects changes)
        var saveResult = await _matchRepository.SaveChangesAsync();
        
        if (saveResult.IsError)
            return saveResult.Errors;

        return Result.Success;
    }
}
