using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Domain.Common;
using TypowanieMeczy.Domain.Entities;

namespace TypowanieMeczy.Domain.Services;

public interface IMatchService
{
    Task<bool> CanStartMatchAsync(MatchId matchId);
    Task<bool> CanFinishMatchAsync(MatchId matchId);
    Task StartMatchAsync(MatchId matchId);
    Task FinishMatchAsync(MatchId matchId, MatchResult result);
    Task<IEnumerable<Match>> GetMatchesNeedingStartAsync();
    Task<IEnumerable<Match>> GetMatchesNeedingFinishAsync();
    Task<bool> IsMatchInProgressAsync(MatchId matchId);
    Task<bool> IsMatchFinishedAsync(MatchId matchId);
} 