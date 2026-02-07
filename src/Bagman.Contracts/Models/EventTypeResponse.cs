namespace Bagman.Contracts.Models;

public record EventTypeResponse(
    Guid Id,
    string Code,
    string Name,
    DateTime StartDate,
    bool IsActive);
