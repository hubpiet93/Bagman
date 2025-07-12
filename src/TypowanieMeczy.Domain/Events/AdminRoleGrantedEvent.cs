using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Events;

public class AdminRoleGrantedEvent : IDomainEvent
{
    public UserId UserId { get; }
    public TableId TableId { get; }
    public DateTime OccurredOn { get; }

    public AdminRoleGrantedEvent(UserId userId, TableId tableId)
    {
        UserId = userId;
        TableId = tableId;
        OccurredOn = DateTime.UtcNow;
    }

    public AdminRoleGrantedEvent(UserId userId, TableId tableId, DateTime occurredOn)
    {
        UserId = userId;
        TableId = tableId;
        OccurredOn = occurredOn;
    }
} 