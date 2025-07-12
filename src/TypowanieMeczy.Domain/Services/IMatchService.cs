using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Services;

public interface IMatchService
{
    Task<Match> CreateMatchAsync(TableId tableId, Country country1, Country country2, MatchDateTime matchDateTime, UserId createdBy);
    Task<Match> UpdateMatchResultAsync(MatchId matchId, MatchResult result, UserId adminUserId);
    Task DeleteMatchAsync(MatchId matchId, UserId adminUserId);
    Task<IEnumerable<Match>> GetTableMatchesAsync(TableId tableId, UserId userId);
    Task<IEnumerable<Match>> GetUpcomingMatchesAsync(TableId tableId, UserId userId);
    Task<IEnumerable<Match>> GetFinishedMatchesAsync(TableId tableId, UserId userId);
    Task<Match> GetMatchDetailsAsync(MatchId matchId, UserId userId);
    Task<bool> CanUserEditMatchAsync(MatchId matchId, UserId userId);
    Task<bool> CanUserDeleteMatchAsync(MatchId matchId, UserId userId);
    Task StartMatchesAsync(); // Background service method
    Task FinishMatchesAsync(); // Background service method
} 