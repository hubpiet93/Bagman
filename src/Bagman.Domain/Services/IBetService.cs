using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Domain.Services;

public interface IBetService
{
    Task<ErrorOr<Bet>> PlaceBetAsync(Guid matchId, Guid userId, string prediction);
    Task<ErrorOr<Bet>> GetBetAsync(Guid matchId, Guid userId);
    Task<ErrorOr<Success>> DeleteBetAsync(Guid matchId, Guid userId);
}

public class BetService : IBetService
{
    private readonly IBetRepository _betRepository;
    private readonly IMatchRepository _matchRepository;

    public BetService(IBetRepository betRepository, IMatchRepository matchRepository)
    {
        _betRepository = betRepository;
        _matchRepository = matchRepository;
    }

    public async Task<ErrorOr<Bet>> PlaceBetAsync(Guid matchId, Guid userId, string prediction)
    {
        // Verify match exists and is not started
        var matchResult = await _matchRepository.GetByIdAsync(matchId);
        if (matchResult.IsError)
            return matchResult.Errors;

        if (matchResult.Value == null)
            return Error.NotFound("Match.NotFound", "Mecz nie został znaleziony");

        if (matchResult.Value.Started)
            return Error.Failure("Match.AlreadyStarted", "Nie można typować na mecz, który już się rozpoczął");

        // Check if user already has a bet
        var existingBetResult = await _betRepository.GetByUserAndMatchAsync(userId, matchId);
        if (existingBetResult.IsError)
            return existingBetResult.Errors;

        if (existingBetResult.Value != null)
        {
            // Update existing bet
            existingBetResult.Value.Prediction = prediction;
            existingBetResult.Value.EditedAt = DateTime.UtcNow;
            return await _betRepository.UpdateAsync(existingBetResult.Value);
        }

        // Create new bet
        var bet = new Bet
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            MatchId = matchId,
            Prediction = prediction,
            EditedAt = DateTime.UtcNow
        };

        return await _betRepository.CreateAsync(bet);
    }

    public async Task<ErrorOr<Bet>> GetBetAsync(Guid matchId, Guid userId)
    {
        var betResult = await _betRepository.GetByUserAndMatchAsync(userId, matchId);
        if (betResult.IsError)
            return betResult.Errors;

        if (betResult.Value == null)
            return Error.NotFound("Bet.NotFound", "Typ nie został znaleziony");

        return betResult.Value;
    }

    public async Task<ErrorOr<Success>> DeleteBetAsync(Guid matchId, Guid userId)
    {
        // Verify match exists and is not started
        var matchResult = await _matchRepository.GetByIdAsync(matchId);
        if (matchResult.IsError)
            return matchResult.Errors;

        if (matchResult.Value == null)
            return Error.NotFound("Match.NotFound", "Mecz nie został znaleziony");

        if (matchResult.Value.Started)
            return Error.Failure("Match.AlreadyStarted", "Nie można usunąć typu na mecz, który już się rozpoczął");

        // Get bet
        var betResult = await _betRepository.GetByUserAndMatchAsync(userId, matchId);
        if (betResult.IsError)
            return betResult.Errors;

        if (betResult.Value == null)
            return Error.NotFound("Bet.NotFound", "Typ nie został znaleziony");

        return await _betRepository.DeleteAsync(betResult.Value.Id);
    }
}
