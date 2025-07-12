using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Events;

public class UserCreatedEvent : IDomainEvent
{
    public UserId UserId { get; }
    public Login Login { get; }
    public Email Email { get; }
    public DateTime OccurredOn { get; }

    public UserCreatedEvent(UserId userId, Login login, Email email)
    {
        UserId = userId;
        Login = login;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }
} 