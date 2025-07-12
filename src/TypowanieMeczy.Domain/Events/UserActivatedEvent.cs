using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Events;

public class UserActivatedEvent : IDomainEvent
{
    public UserId UserId { get; }
    public DateTime OccurredOn { get; }

    public UserActivatedEvent(UserId userId)
    {
        UserId = userId;
        OccurredOn = DateTime.UtcNow;
    }
} 