using TypowanieMeczy.Domain.Common;
using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.Services;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Domain.Interfaces;

namespace TypowanieMeczy.Infrastructure.Services;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly ITableRepository _tableRepository;

    public MatchService(IMatchRepository matchRepository, ITableRepository tableRepository)
    {
        _matchRepository = matchRepository;
        _tableRepository = tableRepository;
    }

    public async Task<Match> CreateMatchAsync(TableId tableId, Country country1, Country country2, MatchDateTime matchDateTime, UserId createdBy)
    {
        var table = await _tableRepository.GetByIdAsync(tableId) ?? throw new Exception("Table not found");
        var match = new Match(tableId, country1, country2, matchDateTime, createdBy);
        table.AddMatch(match);
        await _matchRepository.AddAsync(match);
        await _tableRepository.UpdateAsync(table);
        return match;
    }

    public async Task<Match> UpdateMatchResultAsync(MatchId matchId, MatchResult result, UserId adminUserId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId) ?? throw new Exception("Match not found");
        match.FinishMatch(result);
        await _matchRepository.UpdateAsync(match);
        return match;
    }

    public async Task DeleteMatchAsync(MatchId matchId, UserId adminUserId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId) ?? throw new Exception("Match not found");
        await _matchRepository.DeleteAsync(matchId);
    }

    public async Task<IEnumerable<Match>> GetTableMatchesAsync(TableId tableId, UserId userId)
    {
        return await _matchRepository.GetByTableIdAsync(tableId);
    }

    public async Task<IEnumerable<Match>> GetUpcomingMatchesAsync(TableId tableId, UserId userId)
    {
        return (await _matchRepository.GetByTableIdAsync(tableId)).Where(m => m.MatchDateTime.Value > DateTime.UtcNow);
    }

    public async Task<IEnumerable<Match>> GetFinishedMatchesAsync(TableId tableId, UserId userId)
    {
        return (await _matchRepository.GetByTableIdAsync(tableId)).Where(m => m.Status == MatchStatus.Finished);
    }

    public async Task<Match> GetMatchDetailsAsync(MatchId matchId, UserId userId)
    {
        return await _matchRepository.GetByIdAsync(matchId) ?? throw new Exception("Match not found");
    }

    public Task<bool> CanUserEditMatchAsync(MatchId matchId, UserId userId)
    {
        // Implement logic based on domain rules
        throw new NotImplementedException();
    }

    public Task<bool> CanUserDeleteMatchAsync(MatchId matchId, UserId userId)
    {
        // Implement logic based on domain rules
        throw new NotImplementedException();
    }

    public Task StartMatchesAsync()
    {
        // Implement background logic
        throw new NotImplementedException();
    }

    public Task FinishMatchesAsync()
    {
        // Implement background logic
        throw new NotImplementedException();
    }
} 