using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.Services;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Domain.Interfaces;
using TypowanieMeczy.Domain.Common;

namespace TypowanieMeczy.Infrastructure.Services;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IBetRepository _betRepository;
    private readonly IPoolService _poolService;

    public MatchService(IMatchRepository matchRepository, IBetRepository betRepository, IPoolService poolService)
    {
        _matchRepository = matchRepository;
        _betRepository = betRepository;
        _poolService = poolService;
    }

    public async Task<bool> CanStartMatchAsync(MatchId matchId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        return match != null && !match.IsStarted && match.MatchDateTime.Value <= DateTime.UtcNow;
    }

    public async Task<bool> CanFinishMatchAsync(MatchId matchId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        return match != null && match.IsStarted && match.Status == MatchStatus.InProgress;
    }

    public async Task StartMatchAsync(MatchId matchId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null) throw new InvalidOperationException("Match not found");

        if (match.IsStarted) throw new InvalidOperationException("Match already started");

        match.StartMatch();
        await _matchRepository.UpdateAsync(match);
    }

    public async Task FinishMatchAsync(MatchId matchId, MatchResult result)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null) throw new InvalidOperationException("Match not found");

        if (!match.IsStarted) throw new InvalidOperationException("Match not started");

        match.FinishMatch(result);
        await _matchRepository.UpdateAsync(match);

        // Process pool distribution
        var winners = await _betRepository.GetWinnersByMatchIdAsync(matchId);
        var winnerIds = winners.Select(w => w.UserId);
        await _poolService.DistributePoolAsync(matchId, winnerIds);
    }

    public async Task<IEnumerable<Match>> GetMatchesNeedingStartAsync()
    {
        return await _matchRepository.GetMatchesNeedingStartAsync();
    }

    public async Task<IEnumerable<Match>> GetMatchesNeedingFinishAsync()
    {
        return await _matchRepository.GetMatchesNeedingFinishAsync();
    }

    public async Task<bool> IsMatchInProgressAsync(MatchId matchId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        return match != null && match.IsStarted && match.Status == MatchStatus.InProgress;
    }

    public async Task<bool> IsMatchFinishedAsync(MatchId matchId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        return match != null && match.Status == MatchStatus.Finished;
    }
} 