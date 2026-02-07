namespace Bagman.Application.Features.Tables.GetTableDashboard;

public record LeaderboardEntryResult
{
    public required int Position { get; init; }
    public required Guid UserId { get; init; }
    public required string Login { get; init; }
    public required int Points { get; init; }
    public required int ExactHits { get; init; }
    public required int WinnerHits { get; init; }
    public required int TotalBets { get; init; }
    public required double Accuracy { get; init; }
}
