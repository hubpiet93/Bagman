using TypowanieMeczy.Domain.Common;
using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Interfaces;

public interface IMatchRepository
{
    Task<Match?> GetByIdAsync(MatchId id);
    Task<IEnumerable<Match>> GetByTableIdAsync(TableId tableId);
    Task<IEnumerable<Match>> GetUpcomingByTableIdAsync(TableId tableId);
    Task<IEnumerable<Match>> GetFinishedByTableIdAsync(TableId tableId);
    Task<IEnumerable<Match>> GetByTableIdAndStatusAsync(TableId tableId, MatchStatus status);
    Task AddAsync(Match match);
    Task UpdateAsync(Match match);
    Task DeleteAsync(MatchId id);
    Task<bool> ExistsAsync(MatchId id);
    Task<IEnumerable<Match>> GetMatchesNeedingStartAsync();
    Task<IEnumerable<Match>> GetMatchesNeedingFinishAsync();
} 