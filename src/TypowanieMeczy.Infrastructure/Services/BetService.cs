using TypowanieMeczy.Domain.Services;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Domain.Interfaces;

namespace TypowanieMeczy.Infrastructure.Services;

public class BetService : IBetService
{
    private readonly IBetRepository _betRepository;
    private readonly IMatchRepository _matchRepository;

    public BetService(IBetRepository betRepository, IMatchRepository matchRepository)
    {
        _betRepository = betRepository;
        _matchRepository = matchRepository;
    }

    public async Task<bool> CanUserBetAsync(UserId userId, MatchId matchId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null || match.IsStarted) return false;
        
        var existingBet = await _betRepository.GetByUserAndMatchAsync(userId, matchId);
        return existingBet == null;
    }

    public async Task<bool> IsBettingClosedAsync(MatchId matchId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        return match == null || match.IsStarted;
    }

    public async Task<IEnumerable<UserId>> GetWinnersAsync(MatchId matchId, MatchResult actualResult)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null || match.Result == null)
            return Enumerable.Empty<UserId>();
        return match.Bets.Where(b => b.Prediction.Value == actualResult.Value).Select(b => b.UserId);
    }

    public async Task<decimal> CalculateWinningsAsync(MatchId matchId, UserId userId)
    {
        // Implement logic for calculating winnings
        await Task.CompletedTask; // Placeholder for async operation
        return 0;
    }
} 