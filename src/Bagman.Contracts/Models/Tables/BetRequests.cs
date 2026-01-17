namespace Bagman.Contracts.Models.Tables;

public record PlaceBetRequest
{
    public required string Prediction { get; init; }
}

public record BetResponse
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required Guid MatchId { get; init; }
    public required string Prediction { get; init; }
    public required DateTime EditedAt { get; init; }
}
