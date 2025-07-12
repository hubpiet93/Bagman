using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Events;

public class MatchFinishedEvent : IDomainEvent
{
    public MatchId MatchId { get; }
    public MatchResult Result { get; }
    public DateTime OccurredOn { get; }

    public MatchFinishedEvent(MatchId matchId, MatchResult result)
    {
        MatchId = matchId;
        Result = result;
        OccurredOn = DateTime.UtcNow;
    }
} 