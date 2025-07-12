using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Exceptions;

public class TableNotFoundException : DomainException
{
    public TableId TableId { get; }

    public TableNotFoundException(TableId tableId) 
        : base($"Table with ID {tableId} was not found.")
    {
        TableId = tableId;
    }
} 