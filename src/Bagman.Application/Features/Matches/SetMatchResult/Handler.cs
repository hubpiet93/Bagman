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
    private readonly IUserRepository _userRepository;

    public SetMatchResultHandler(IMatchRepository matchRepository, IUserRepository userRepository)
    {
        _matchRepository = matchRepository;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Success>> HandleAsync(
        SetMatchResultCommand request,
        CancellationToken cancellationToken = default)
    {
        // Verify user is SuperAdmin
        var userResult = await _userRepository.GetByIdAsync(request.UserId);
        if (userResult.IsError)
            return userResult.Errors;

        if (userResult.Value == null)
            return Error.NotFound("User.NotFound", "Użytkownik nie został znaleziony");

        if (!userResult.Value.IsSuperAdmin)
            return Error.Forbidden("User.NotSuperAdmin", "Nie masz uprawnień do zarządzania meczami");

        // Get match aggregate
        var matchResult = await _matchRepository.GetByIdAsync(request.MatchId);
        if (matchResult.IsError)
            return matchResult.Errors;

        if (matchResult.Value == null)
            return Error.NotFound("Match.NotFound", "Mecz nie został znaleziony");

        var match = matchResult.Value;

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
