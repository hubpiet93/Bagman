using TypowanieMeczy.Domain.Services;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Domain.Interfaces;
using TypowanieMeczy.Domain.Entities;

namespace TypowanieMeczy.Infrastructure.Services;

public class BetService : IBetService
{
    private readonly IBetRepository _betRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IPoolRepository _poolRepository;

    public BetService(IBetRepository betRepository, IMatchRepository matchRepository, IPoolRepository poolRepository)
    {
        _betRepository = betRepository;
        _matchRepository = matchRepository;
        _poolRepository = poolRepository;
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
        var pool = await _poolRepository.GetByMatchIdAsync(matchId);
        if (pool == null) return 0;

        if (pool.Winners.Contains(userId))
        {
            var winnerCount = pool.Winners.Count;
            return winnerCount > 0 ? pool.Amount / winnerCount : 0;
        }

        return 0;
    }

    public async Task PlaceBetAsync(Bet bet)
    {
        var existingBet = await _betRepository.GetByUserAndMatchAsync(bet.UserId, bet.MatchId);
        if (existingBet != null)
        {
            existingBet.UpdatePrediction(bet.Prediction);
            await _betRepository.UpdateAsync(existingBet);
        }
        else
        {
            await _betRepository.AddAsync(bet);
        }
    }

    public async Task<IEnumerable<Bet>> GetMatchBetsAsync(MatchId matchId)
    {
        return await _betRepository.GetByMatchIdAsync(matchId);
    }
} 