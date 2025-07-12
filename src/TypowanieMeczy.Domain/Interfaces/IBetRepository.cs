using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Interfaces;

public interface IBetRepository
{
    Task<Bet?> GetByIdAsync(BetId id);
    Task<IEnumerable<Bet>> GetByMatchIdAsync(MatchId matchId);
    Task<IEnumerable<Bet>> GetByUserIdAsync(UserId userId);
    Task<Bet?> GetByUserAndMatchAsync(UserId userId, MatchId matchId);
    Task<IEnumerable<Bet>> GetWinnersByMatchIdAsync(MatchId matchId);
    Task AddAsync(Bet bet);
    Task UpdateAsync(Bet bet);
    Task DeleteAsync(BetId id);
} 