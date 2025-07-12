using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Exceptions;

public class MatchNotFoundException : DomainException
{
    public MatchId MatchId { get; }

    public MatchNotFoundException(MatchId matchId) 
        : base($"Match with ID {matchId} was not found.")
    {
        MatchId = matchId;
    }
} 