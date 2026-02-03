namespace Bagman.Contracts.Models;

public record MatchCreatedResponse(
    Guid Id,
    Guid EventTypeId,
    string Country1,
    string Country2,
    DateTime MatchDateTime,
    string Status,
    DateTime CreatedAt);
