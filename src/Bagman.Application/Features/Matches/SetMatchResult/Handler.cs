using Bagman.Application.Common;
using Bagman.Domain.Common.ValueObjects;
using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using Bagman.Domain.Services;
using Bagman.Domain.ValueObjects;
using Bagman.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Application.Features.Matches.SetMatchResult;

public record SetMatchResultCommand
{
    public required Guid MatchId { get; init; }
    public required string Result { get; init; }
    public required Guid UserId { get; init; }
}

public record SetMatchResultResult
{
    public required Guid Id { get; init; }
    public required string Result { get; init; }
    public required string Status { get; init; }
}

public class SetMatchResultHandler : IFeatureHandler<SetMatchResultCommand, SetMatchResultResult>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDbContext _dbContext;

    public SetMatchResultHandler(
        IMatchRepository matchRepository,
        IUserRepository userRepository,
        ApplicationDbContext dbContext)
    {
        _matchRepository = matchRepository;
        _userRepository = userRepository;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<SetMatchResultResult>> HandleAsync(
        SetMatchResultCommand request,
        CancellationToken cancellationToken = default)
    {
        // Verify user is SuperAdmin
        var userResult = await _userRepository.GetByIdAsync(request.UserId);
        if (userResult.IsError)
            return userResult.Errors;

        if (userResult.Value == null)
            return Error.NotFound("User.NotFound", "Użytkownik nie został znaleziony");

        if (!userResult.Value.IsSuperAdmin)
            return Error.Forbidden("User.NotSuperAdmin", "Nie masz uprawnień do zarządzania meczami");

        // Get match aggregate
        var matchResult = await _matchRepository.GetByIdAsync(request.MatchId);
        if (matchResult.IsError)
            return matchResult.Errors;

        if (matchResult.Value == null)
            return Error.NotFound("Match.NotFound", "Mecz nie został znaleziony");

        var match = matchResult.Value;

        // Create value object
        var scoreResult = Score.Create(request.Result);
        if (scoreResult.IsError)
            return scoreResult.Errors;

        // Set result through aggregate (validates Started && not finished)
        var setResultResult = match.SetResult(scoreResult.Value);
        if (setResultResult.IsError)
            return setResultResult.Errors;

        // Persist match result
        var saveResult = await _matchRepository.SaveChangesAsync();
        if (saveResult.IsError)
            return saveResult.Errors;

        // Settle pools for this match
        await SettlePoolsAsync(match, cancellationToken);

        return new SetMatchResultResult
        {
            Id = match.Id,
            Result = match.Result!.Value,
            Status = match.Status
        };
    }

    private async Task SettlePoolsAsync(Match match, CancellationToken cancellationToken)
    {
        var pools = await _dbContext.Pools
            .Where(p => p.MatchId == match.Id && p.Status == "active")
            .AsTracking()
            .ToListAsync(cancellationToken);

        if (!pools.Any())
            return;

        var matchResultValue = match.Result!.Value;

        // Get all bets for this match
        var bets = match.Bets;

        // Find winners: first try exact hits, then winner hits
        var exactHitUserIds = bets
            .Where(b => BetScoringService.CalculateResult(b.Prediction.Value, matchResultValue).Type ==
                        BetResultType.ExactHit)
            .Select(b => b.UserId)
            .ToList();

        var winnerHitUserIds = bets
            .Where(b => BetScoringService.CalculateResult(b.Prediction.Value, matchResultValue).Type ==
                        BetResultType.WinnerHit)
            .Select(b => b.UserId)
            .ToList();

        var winnerUserIds = exactHitUserIds.Any() ? exactHitUserIds : winnerHitUserIds;

        foreach (var pool in pools)
        {
            if (winnerUserIds.Any())
            {
                var amountPerWinner = pool.Amount / winnerUserIds.Count;

                foreach (var winnerId in winnerUserIds)
                {
                    var poolWinner = new PoolWinner
                    {
                        PoolId = pool.Id,
                        UserId = winnerId,
                        AmountWon = amountPerWinner
                    };
                    _dbContext.PoolWinners.Add(poolWinner);

                    // Update user stats for tables linked to this event type
                    await UpdateWinnerStatsAsync(winnerId, match.EventTypeId, amountPerWinner, cancellationToken);
                }

                pool.Status = "won";
            }
            else
            {
                pool.Status = "rollover";
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateWinnerStatsAsync(
        Guid userId, Guid eventTypeId, decimal amountWon, CancellationToken cancellationToken)
    {
        // Find all tables linked to this event type
        var tableIds = await _dbContext.Tables
            .Where(t => t.EventTypeId == eventTypeId)
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);

        foreach (var tableId in tableIds)
        {
            var stats = await _dbContext.UserStats
                .AsTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId && s.TableId == tableId, cancellationToken);

            if (stats != null)
            {
                stats.PoolsWon++;
                stats.TotalWon += amountWon;
                stats.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _dbContext.UserStats.Add(new UserStats
                {
                    UserId = userId,
                    TableId = tableId,
                    PoolsWon = 1,
                    TotalWon = amountWon,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }
    }
}
