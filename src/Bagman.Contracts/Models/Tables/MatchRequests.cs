namespace Bagman.Contracts.Models.Tables;

public record CreateMatchRequest
{
    public required string Country1 { get; init; }
    public required string Country2 { get; init; }
    public required DateTime MatchDateTime { get; init; }
}

public record UpdateMatchRequest
{
    public required string Country1 { get; init; }
    public required string Country2 { get; init; }
    public required DateTime MatchDateTime { get; init; }
}

public record SetMatchResultRequest
{
    public required string Result { get; init; }
}

public record MatchResponse
{
    public required Guid Id { get; init; }
    public required Guid EventTypeId { get; init; }
    public required string Country1 { get; init; }
    public required string Country2 { get; init; }
    public required DateTime MatchDateTime { get; init; }
    public required string? Result { get; init; }
    public required string Status { get; init; }
    public required bool Started { get; init; }
    public required DateTime CreatedAt { get; init; }
}
