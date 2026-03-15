using Bagman.Application.Common;
using Bagman.Domain.Services;
using Bagman.Domain.ValueObjects;
using Bagman.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Application.Features.Tables.GetTableDashboard;

public record GetTableDashboardQuery
{
    public required Guid TableId { get; init; }
    public required Guid UserId { get; init; }
}

public record TableDashboardResult
{
    public required Guid TableId { get; init; }
    public required string TableName { get; init; }
    public required int MaxPlayers { get; init; }
    public required decimal Stake { get; init; }
    public required DateTime TableCreatedAt { get; init; }
    public required List<MemberDetailResult> Members { get; init; }
    public required List<MatchDetailResult> Matches { get; init; }
    public required List<BetDetailResult> Bets { get; init; }
    public required List<PoolDetailResult> Pools { get; init; }
    public required List<StatsDetailResult> Stats { get; init; }
    public required List<LeaderboardEntryResult> Leaderboard { get; init; }
}

public record MemberDetailResult
{
    public required Guid UserId { get; init; }
    public required string Login { get; init; }
    public required bool IsAdmin { get; init; }
    public required DateTime JoinedAt { get; init; }
}

public record MatchDetailResult
{
    public required Guid Id { get; init; }
    public required string Country1 { get; init; }
    public required string Country2 { get; init; }
    public required DateTime MatchDateTime { get; init; }
    public string? Result { get; init; }
    public required bool IsStarted { get; init; }
}

public record BetDetailResult
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required Guid MatchId { get; init; }
    public required string Prediction { get; init; }
    public required DateTime EditedAt { get; init; }
}

public record PoolDetailResult
{
    public required Guid MatchId { get; init; }
    public required decimal Amount { get; init; }
    public required string Status { get; init; }
    public List<Guid>? Winners { get; init; }
}

public record StatsDetailResult
{
    public required Guid UserId { get; init; }
    public required int MatchesPlayed { get; init; }
    public required int BetsPlaced { get; init; }
    public required int PoolsWon { get; init; }
    public required decimal TotalWon { get; init; }
}

public class GetTableDashboardHandler : IFeatureHandler<GetTableDashboardQuery, TableDashboardResult>
{
    private readonly ApplicationDbContext _dbContext;

    public GetTableDashboardHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<TableDashboardResult>> HandleAsync(
        GetTableDashboardQuery request,
        CancellationToken cancellationToken = default)
    {
        // Get table and verify it exists
        var table = await _dbContext.Tables
            .Where(t => t.Id == request.TableId)
            .FirstOrDefaultAsync(cancellationToken);

        if (table == null)
            return Error.NotFound("Table.NotFound", "Stół nie został znaleziony");

        // Verify user is table member
        if (!table.IsUserMember(request.UserId))
            return Error.Forbidden("Table.AccessDenied", "Nie masz dostępu do tego stołu");

        var members = await GetTableMembersAsync(request.TableId, cancellationToken);
        var matches = await GetMatchesAsync(table.EventTypeId, cancellationToken);

        var matchIds = matches.Select(m => m.Id).ToList();
        var bets = await GetBetsAsync(matchIds, cancellationToken);

        var memberUserIds = members.Select(m => m.UserId).ToHashSet();
        var stake = table.Stake.Amount;

        var pools = ComputePools(matches, bets, memberUserIds, stake);
        var stats = ComputeStats(members, matches, bets, pools);
        var leaderboard = CalculateLeaderboard(members, matches, bets);

        return new TableDashboardResult
        {
            TableId = table.Id,
            TableName = table.Name.Value,
            MaxPlayers = table.MaxPlayers,
            Stake = stake,
            TableCreatedAt = table.CreatedAt,
            Members = members,
            Matches = matches,
            Bets = bets,
            Pools = pools,
            Stats = stats,
            Leaderboard = leaderboard
        };
    }

    private async Task<List<MemberDetailResult>> GetTableMembersAsync(
        Guid tableId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Tables
            .Where(t => t.Id == tableId)
            .SelectMany(t => t.Members)
            .OrderBy(m => m.JoinedAt)
            .Select(m => new MemberDetailResult
            {
                UserId = m.UserId,
                Login = m.User!.Login,
                IsAdmin = m.IsAdmin,
                JoinedAt = m.JoinedAt
            })
            .ToListAsync(cancellationToken);
    }

    private async Task<List<MatchDetailResult>> GetMatchesAsync(
        Guid eventTypeId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Matches
            .Where(m => m.EventTypeId == eventTypeId)
            .Include(m => m.Bets)
            .OrderBy(m => m.MatchDateTime)
            .Select(m => new MatchDetailResult
            {
                Id = m.Id,
                Country1 = m.Country1.Name,
                Country2 = m.Country2.Name,
                MatchDateTime = m.MatchDateTime,
                Result = m.Result != null ? m.Result.Value : null,
                IsStarted = m.Started
            })
            .ToListAsync(cancellationToken);
    }

    private async Task<List<BetDetailResult>> GetBetsAsync(
        List<Guid> matchIds,
        CancellationToken cancellationToken)
    {
        if (!matchIds.Any())
            return new List<BetDetailResult>();

        return await _dbContext.Matches
            .Where(m => matchIds.Contains(m.Id))
            .Include(m => m.Bets)
            .SelectMany(m => m.Bets)
            .Select(b => new BetDetailResult
            {
                Id = b.Id,
                UserId = b.UserId,
                MatchId = b.MatchId,
                Prediction = b.Prediction.Value,
                EditedAt = b.EditedAt
            })
            .ToListAsync(cancellationToken);
    }

    private static List<PoolDetailResult> ComputePools(
        List<MatchDetailResult> matches,
        List<BetDetailResult> bets,
        HashSet<Guid> memberUserIds,
        decimal stake)
    {
        var memberBetsByMatch = bets
            .Where(b => memberUserIds.Contains(b.UserId))
            .GroupBy(b => b.MatchId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var pools = new List<PoolDetailResult>();
        decimal runningRollover = 0;

        foreach (var match in matches.OrderBy(m => m.MatchDateTime))
        {
            var matchBets = memberBetsByMatch.GetValueOrDefault(match.Id, new List<BetDetailResult>());
            decimal poolAmount = memberUserIds.Count * stake + runningRollover;

            if (poolAmount == 0)
                continue;

            string status;
            List<Guid> winners = new();

            if (match.Result == null)
            {
                status = "active";
            }
            else
            {
                winners = matchBets
                    .Where(b => BetScoringService.CalculateResult(b.Prediction, match.Result).Type == BetResultType.ExactHit)
                    .Select(b => b.UserId)
                    .ToList();

                if (winners.Any())
                {
                    status = "won";
                    runningRollover = 0;
                }
                else
                {
                    status = "rollover";
                    runningRollover = poolAmount;
                }
            }

            pools.Add(new PoolDetailResult
            {
                MatchId = match.Id,
                Amount = poolAmount,
                Status = status,
                Winners = winners.Any() ? winners : null
            });
        }

        return pools;
    }

    private static List<StatsDetailResult> ComputeStats(
        List<MemberDetailResult> members,
        List<MatchDetailResult> matches,
        List<BetDetailResult> bets,
        List<PoolDetailResult> pools)
    {
        var finishedMatchIds = matches
            .Where(m => m.Result != null)
            .Select(m => m.Id)
            .ToHashSet();

        var wonPools = pools.Where(p => p.Status == "won").ToList();

        return members.Select(member =>
        {
            var memberBets = bets.Where(b => b.UserId == member.UserId).ToList();
            var betsPlaced = memberBets.Count;
            var matchesPlayed = memberBets
                .Select(b => b.MatchId)
                .Distinct()
                .Count(mid => finishedMatchIds.Contains(mid));

            var poolsWon = wonPools.Count(p => p.Winners != null && p.Winners.Contains(member.UserId));
            var totalWon = wonPools
                .Where(p => p.Winners != null && p.Winners.Contains(member.UserId))
                .Sum(p => p.Amount / p.Winners!.Count);

            return new StatsDetailResult
            {
                UserId = member.UserId,
                MatchesPlayed = matchesPlayed,
                BetsPlaced = betsPlaced,
                PoolsWon = poolsWon,
                TotalWon = totalWon
            };
        }).ToList();
    }

    private List<LeaderboardEntryResult> CalculateLeaderboard(
        List<MemberDetailResult> members,
        List<MatchDetailResult> matches,
        List<BetDetailResult> bets)
    {
        // Słownik: matchId -> result
        var matchResults = matches
            .Where(m => m.Result != null)
            .ToDictionary(m => m.Id, m => m.Result!);

        // Grupowanie zakładów po użytkowniku
        var userBets = bets.GroupBy(b => b.UserId);

        var leaderboard = new List<LeaderboardEntryResult>();

        foreach (var group in userBets)
        {
            var userId = group.Key;
            var member = members.FirstOrDefault(m => m.UserId == userId);
            if (member == null) continue;

            var points = 0;
            var exactHits = 0;
            var winnerHits = 0;
            var totalBets = 0;

            foreach (var bet in group)
            {
                if (!matchResults.TryGetValue(bet.MatchId, out var result))
                    continue; // Mecz jeszcze nie zakończony

                totalBets++;
                var betResult = BetScoringService.CalculateResult(bet.Prediction, result);

                points += betResult.Points;
                if (betResult.Type == BetResultType.ExactHit) exactHits++;
                if (betResult.Type == BetResultType.WinnerHit) winnerHits++;
            }

            var accuracy = totalBets > 0
                ? Math.Round((exactHits + winnerHits) * 100.0 / totalBets, 1)
                : 0;

            leaderboard.Add(new LeaderboardEntryResult
            {
                Position = 0, // Will be set during sorting
                UserId = userId,
                Login = member.Login,
                Points = points,
                ExactHits = exactHits,
                WinnerHits = winnerHits,
                TotalBets = totalBets,
                Accuracy = accuracy
            });
        }

        // Sortowanie i przypisanie pozycji
        return leaderboard
            .OrderByDescending(e => e.Points)
            .ThenByDescending(e => e.ExactHits)
            .ThenByDescending(e => e.Accuracy)
            .Select((e, i) => e with {Position = i + 1})
            .ToList();
    }
}
