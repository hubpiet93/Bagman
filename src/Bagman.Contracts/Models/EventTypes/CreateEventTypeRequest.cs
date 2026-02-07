namespace Bagman.Contracts.Models.EventTypes;

public record CreateEventTypeRequest
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required DateTime StartDate { get; init; }
}
