using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Events;

public class TableCreatedEvent : IDomainEvent
{
    public TableId TableId { get; }
    public TableName Name { get; }
    public UserId CreatedBy { get; }
    public DateTime OccurredOn { get; }

    public TableCreatedEvent(TableId tableId, TableName name, UserId createdBy)
    {
        TableId = tableId;
        Name = name;
        CreatedBy = createdBy;
        OccurredOn = DateTime.UtcNow;
    }
} 