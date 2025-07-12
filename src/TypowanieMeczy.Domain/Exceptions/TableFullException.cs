namespace TypowanieMeczy.Domain.Exceptions;

public class TableFullException : DomainException
{
    public TableFullException(string message) : base(message) { }
} 