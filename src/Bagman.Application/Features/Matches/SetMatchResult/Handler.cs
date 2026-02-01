using Bagman.Application.Common;
using Bagman.Domain.Common.ValueObjects;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.Matches.SetMatchResult;

public record SetMatchResultCommand
{
    public required Guid MatchId { get; init; }
    public required string Result { get; init; }
    public required Guid UserId { get; init; }
}

public class SetMatchResultHandler : IFeatureHandler<SetMatchResultCommand, Success>
{
    private readonly IMatchRepository _matchRepository;
    private readonly ITableRepository _tableRepository;

    public SetMatchResultHandler(IMatchRepository matchRepository, ITableRepository tableRepository)
    {
        _matchRepository = matchRepository;
        _tableRepository = tableRepository;
    }

    public async Task<ErrorOr<Success>> HandleAsync(
        SetMatchResultCommand request,
        CancellationToken cancellationToken = default)
    {
        // Get match aggregate
        var matchResult = await _matchRepository.GetByIdAsync(request.MatchId);
        if (matchResult.IsError)
            return matchResult.Errors;

        if (matchResult.Value == null)
            return Error.NotFound("Match.NotFound", "Mecz nie został znaleziony");

        var match = matchResult.Value;

        // Verify user is admin
        var tableResult = await _tableRepository.GetByIdAsync(match.TableId);
        if (tableResult.IsError)
            return tableResult.Errors;

        if (tableResult.Value == null)
            return Error.NotFound("Table.NotFound", "Stół nie został znaleziony");

        if (!tableResult.Value.IsUserAdmin(request.UserId))
            return Error.Forbidden("Table.NotAdmin", "Nie masz uprawnień do wykonania tej czynności");

        // Create value object
        var scoreResult = Score.Create(request.Result);
        if (scoreResult.IsError)
            return scoreResult.Errors;

        // Set result through aggregate
        var setResultResult = match.SetResult(scoreResult.Value);
        if (setResultResult.IsError)
            return setResultResult.Errors;

        // Persist changes (EF change tracking automatically detects changes)
        var saveResult = await _matchRepository.SaveChangesAsync();
        
        if (saveResult.IsError)
            return saveResult.Errors;

        return Result.Success;
    }
}
