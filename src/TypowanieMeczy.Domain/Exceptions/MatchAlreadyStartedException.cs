namespace TypowanieMeczy.Domain.Exceptions;

public class MatchAlreadyStartedException : DomainException
{
    public MatchAlreadyStartedException(string message) : base(message)
    {
    }
} 