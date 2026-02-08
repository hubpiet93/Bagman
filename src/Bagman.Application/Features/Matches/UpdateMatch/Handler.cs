using Bagman.Application.Common;
using Bagman.Domain.Common.ValueObjects;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.Matches.UpdateMatch;

public record UpdateMatchCommand
{
    public required Guid MatchId { get; init; }
    public required string Country1 { get; init; }
    public required string Country2 { get; init; }
    public required DateTime MatchDateTime { get; init; }
    public required Guid UserId { get; init; }
}

public class UpdateMatchHandler : IFeatureHandler<UpdateMatchCommand, Success>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;

    public UpdateMatchHandler(IMatchRepository matchRepository, IUserRepository userRepository)
    {
        _matchRepository = matchRepository;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Success>> HandleAsync(
        UpdateMatchCommand request,
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

        // Create value objects
        var country1Result = Country.Create(request.Country1);
        if (country1Result.IsError)
            return country1Result.Errors;

        var country2Result = Country.Create(request.Country2);
        if (country2Result.IsError)
            return country2Result.Errors;

        // Update through aggregate
        var updateResult = match.Update(country1Result.Value, country2Result.Value, request.MatchDateTime);
        if (updateResult.IsError)
            return updateResult.Errors;

        // Persist changes (EF change tracking automatically detects changes)
        var saveResult = await _matchRepository.SaveChangesAsync();

        if (saveResult.IsError)
            return saveResult.Errors;

        return Result.Success;
    }
}
