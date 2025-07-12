using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Events;

public class MatchStartedEvent : IDomainEvent
{
    public MatchId MatchId { get; }
    public DateTime OccurredOn { get; }

    public MatchStartedEvent(MatchId matchId)
    {
        MatchId = matchId;
        OccurredOn = DateTime.UtcNow;
    }
} 