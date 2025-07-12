using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Events;

public class AdminRoleRevokedEvent : IDomainEvent
{
    public UserId UserId { get; }
    public TableId TableId { get; }
    public DateTime OccurredOn { get; }

    public AdminRoleRevokedEvent(UserId userId, TableId tableId)
    {
        UserId = userId;
        TableId = tableId;
        OccurredOn = DateTime.UtcNow;
    }

    public AdminRoleRevokedEvent(UserId userId, TableId tableId, DateTime occurredOn)
    {
        UserId = userId;
        TableId = tableId;
        OccurredOn = occurredOn;
    }
} 