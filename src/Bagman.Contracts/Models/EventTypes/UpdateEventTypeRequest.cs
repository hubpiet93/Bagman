namespace Bagman.Contracts.Models.EventTypes;

public record UpdateEventTypeRequest
{
    public string? Name { get; init; }
    public DateTime? StartDate { get; init; }
}