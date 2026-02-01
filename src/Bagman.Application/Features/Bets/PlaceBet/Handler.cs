using Bagman.Application.Common;
using Bagman.Domain.Common.ValueObjects;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.Bets.PlaceBet;

public record PlaceBetCommand
{
    public required Guid MatchId { get; init; }
    public required Guid UserId { get; init; }
    public required string Prediction { get; init; }
}

public record PlaceBetResult
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required Guid MatchId { get; init; }
    public required string Prediction { get; init; }
    public required DateTime EditedAt { get; init; }
}

public class PlaceBetHandler : IFeatureHandler<PlaceBetCommand, PlaceBetResult>
{
    private readonly IMatchRepository _matchRepository;

    public PlaceBetHandler(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public async Task<ErrorOr<PlaceBetResult>> HandleAsync(
        PlaceBetCommand request,
        CancellationToken cancellationToken = default)
    {
        // Get match aggregate
        var matchResult = await _matchRepository.GetByIdAsync(request.MatchId);
        if (matchResult.IsError)
            return matchResult.Errors;

        if (matchResult.Value == null)
            return Error.NotFound("Match.NotFound", "Mecz nie został znaleziony");

        var match = matchResult.Value;

        // Create value object
        var predictionResult = Prediction.Create(request.Prediction);
        if (predictionResult.IsError)
            return predictionResult.Errors;

        // Place bet through aggregate
        var placeBetResult = match.PlaceBet(request.UserId, predictionResult.Value);
        if (placeBetResult.IsError)
            return placeBetResult.Errors;

        // Persist changes (EF change tracking automatically detects changes)
        var saveResult = await _matchRepository.SaveChangesAsync();
        
        if (saveResult.IsError)
            return saveResult.Errors;

        // Get the bet we just created/updated
        var bet = match.GetUserBet(request.UserId)!;
        
        return new PlaceBetResult
        {
            Id = bet.Id,
            UserId = bet.UserId,
            MatchId = bet.MatchId,
            Prediction = bet.Prediction.Value,
            EditedAt = bet.EditedAt
        };
    }
}
