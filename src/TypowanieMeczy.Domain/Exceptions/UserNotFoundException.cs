using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Exceptions;

public class UserNotFoundException : DomainException
{
    public UserId UserId { get; }

    public UserNotFoundException(UserId userId) 
        : base($"User with ID {userId} was not found.")
    {
        UserId = userId;
    }
} 