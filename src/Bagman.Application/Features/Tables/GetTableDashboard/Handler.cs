using Bagman.Application.Common;
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
    public required Guid Id { get; init; }
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

        // Get table members in parallel
        var members = await GetTableMembersAsync(request.TableId, cancellationToken);

        // Get matches for the table's event type in parallel
        var matches = await GetMatchesAsync(table.EventTypeId, cancellationToken);

        // Get all bets for those matches
        var matchIds = matches.Select(m => m.Id).ToList();
        var bets = await GetBetsAsync(matchIds, cancellationToken);

        // Get pools for those matches
        var pools = await GetPoolsAsync(matchIds, cancellationToken);

        // Get user stats for the table
        var stats = await GetUserStatsAsync(request.TableId, cancellationToken);

        return new TableDashboardResult
        {
            TableId = table.Id,
            TableName = table.Name.Value,
            MaxPlayers = table.MaxPlayers,
            Stake = table.Stake.Amount,
            TableCreatedAt = table.CreatedAt,
            Members = members,
            Matches = matches,
            Bets = bets,
            Pools = pools,
            Stats = stats
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

    private async Task<List<PoolDetailResult>> GetPoolsAsync(
        List<Guid> matchIds,
        CancellationToken cancellationToken)
    {
        if (!matchIds.Any())
            return new List<PoolDetailResult>();

        return await _dbContext.Pools
            .Where(p => matchIds.Contains(p.MatchId))
            .Select(p => new PoolDetailResult
            {
                Id = p.Id,
                MatchId = p.MatchId,
                Amount = p.Amount,
                Status = p.Status,
                Winners = p.Winners.Select(w => w.UserId).ToList()
            })
            .ToListAsync(cancellationToken);
    }

    private async Task<List<StatsDetailResult>> GetUserStatsAsync(
        Guid tableId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.UserStats
            .Where(s => s.TableId == tableId)
            .Select(s => new StatsDetailResult
            {
                UserId = s.UserId,
                MatchesPlayed = s.MatchesPlayed,
                BetsPlaced = s.BetsPlaced,
                PoolsWon = s.PoolsWon,
                TotalWon = s.TotalWon
            })
            .ToListAsync(cancellationToken);
    }
}
