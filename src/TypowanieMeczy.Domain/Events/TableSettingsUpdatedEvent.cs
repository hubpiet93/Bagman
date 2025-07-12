using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Events;

public class TableSettingsUpdatedEvent : IDomainEvent
{
    public TableId TableId { get; }
    public DateTime OccurredOn { get; }

    public TableSettingsUpdatedEvent(TableId tableId)
    {
        TableId = tableId;
        OccurredOn = DateTime.UtcNow;
    }
} 