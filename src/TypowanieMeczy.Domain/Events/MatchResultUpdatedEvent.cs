using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Events;

public class MatchResultUpdatedEvent : IDomainEvent
{
    public MatchId MatchId { get; }
    public MatchResult Result { get; }
    public DateTime OccurredOn { get; }

    public MatchResultUpdatedEvent(MatchId matchId, MatchResult result)
    {
        MatchId = matchId;
        Result = result;
        OccurredOn = DateTime.UtcNow;
    }
} 