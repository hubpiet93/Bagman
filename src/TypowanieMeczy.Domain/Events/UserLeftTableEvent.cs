using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Events;

public class UserLeftTableEvent : IDomainEvent
{
    public UserId UserId { get; }
    public TableId TableId { get; }
    public DateTime OccurredOn { get; }

    public UserLeftTableEvent(UserId userId, TableId tableId)
    {
        UserId = userId;
        TableId = tableId;
        OccurredOn = DateTime.UtcNow;
    }
} 