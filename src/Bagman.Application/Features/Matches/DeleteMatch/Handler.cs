using Bagman.Application.Common;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.Matches.DeleteMatch;

public record DeleteMatchCommand
{
    public required Guid MatchId { get; init; }
    public required Guid UserId { get; init; }
}

public class DeleteMatchHandler : IFeatureHandler<DeleteMatchCommand, Success>
{
    private readonly IMatchRepository _matchRepository;
    private readonly ITableRepository _tableRepository;

    public DeleteMatchHandler(IMatchRepository matchRepository, ITableRepository tableRepository)
    {
        _matchRepository = matchRepository;
        _tableRepository = tableRepository;
    }

    public async Task<ErrorOr<Success>> HandleAsync(
        DeleteMatchCommand request,
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

        // Validate can delete through aggregate
        var deleteValidation = match.Delete();
        if (deleteValidation.IsError)
            return deleteValidation.Errors;

        // Delete
        _matchRepository.Delete(match);
        var saveResult = await _matchRepository.SaveChangesAsync();
        
        if (saveResult.IsError)
            return saveResult.Errors;

        return Result.Success;
    }
}
