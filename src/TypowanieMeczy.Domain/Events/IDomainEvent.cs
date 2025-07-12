namespace TypowanieMeczy.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
} 