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
    private readonly IUserRepository _userRepository;

    public DeleteMatchHandler(IMatchRepository matchRepository, IUserRepository userRepository)
    {
        _matchRepository = matchRepository;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Success>> HandleAsync(
        DeleteMatchCommand request,
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
