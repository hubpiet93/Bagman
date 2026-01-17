namespace Bagman.Contracts.Models.Tables;

public record GrantAdminRequest
{
    public required Guid UserId { get; init; }
}
