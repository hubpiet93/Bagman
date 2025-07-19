namespace Bagman.Contracts.Models;

public record TableDashboardResponse
{
    public required TableInfo Table { get; init; }
    public required List<MemberInfo> Members { get; init; }
    public required List<MatchInfo> Matches { get; init; }
    public required List<BetInfo> Bets { get; init; }
    public required List<PoolInfo> Pools { get; init; }
    public required List<StatsInfo> Stats { get; init; }
}

public record TableInfo
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int MaxPlayers { get; init; }
    public required decimal Stake { get; init; }
    public required DateTime CreatedAt { get; init; }
}

public record MemberInfo
{
    public required Guid UserId { get; init; }
    public required string Login { get; init; }
    public required bool IsAdmin { get; init; }
    public required DateTime JoinedAt { get; init; }
}

public record MatchInfo
{
    public required Guid Id { get; init; }
    public required string Country1 { get; init; }
    public required string Country2 { get; init; }
    public required DateTime MatchDateTime { get; init; }
    public string? Result { get; init; }
    public required bool IsStarted { get; init; }
}

public record BetInfo
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required Guid MatchId { get; init; }
    public required string Prediction { get; init; }
    public required DateTime EditedAt { get; init; }
}

public record PoolInfo
{
    public required Guid Id { get; init; }
    public required Guid MatchId { get; init; }
    public required decimal Amount { get; init; }
    public required string Status { get; init; }
    public List<Guid>? Winners { get; init; }
}

public record StatsInfo
{
    public required Guid UserId { get; init; }
    public required int MatchesPlayed { get; init; }
    public required int BetsPlaced { get; init; }
    public required int PoolsWon { get; init; }
    public required decimal TotalWon { get; init; }
} 