using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Domain.Entities;

namespace TypowanieMeczy.Domain.Services;

public interface IBetService
{
    Task<bool> CanUserBetAsync(UserId userId, MatchId matchId);
    Task<bool> IsBettingClosedAsync(MatchId matchId);
    Task<IEnumerable<UserId>> GetWinnersAsync(MatchId matchId, MatchResult actualResult);
    Task<decimal> CalculateWinningsAsync(MatchId matchId, UserId userId);
    Task PlaceBetAsync(Bet bet);
    Task<IEnumerable<Bet>> GetMatchBetsAsync(MatchId matchId);
} 