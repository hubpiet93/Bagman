using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Events;

public class MatchAddedEvent : IDomainEvent
{
    public MatchId MatchId { get; }
    public TableId TableId { get; }
    public DateTime OccurredOn { get; }

    public MatchAddedEvent(MatchId matchId, TableId tableId)
    {
        MatchId = matchId;
        TableId = tableId;
        OccurredOn = DateTime.UtcNow;
    }

    public MatchAddedEvent(MatchId matchId, TableId tableId, DateTime occurredOn)
    {
        MatchId = matchId;
        TableId = tableId;
        OccurredOn = occurredOn;
    }
} 