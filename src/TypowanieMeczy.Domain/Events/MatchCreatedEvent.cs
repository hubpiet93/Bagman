using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Events;

public class MatchCreatedEvent : IDomainEvent
{
    public MatchId MatchId { get; }
    public TableId TableId { get; }
    public Country Country1 { get; }
    public Country Country2 { get; }
    public MatchDateTime MatchDateTime { get; }
    public DateTime OccurredOn { get; }

    public MatchCreatedEvent(MatchId matchId, TableId tableId, Country country1, Country country2, MatchDateTime matchDateTime)
    {
        MatchId = matchId;
        TableId = tableId;
        Country1 = country1;
        Country2 = country2;
        MatchDateTime = matchDateTime;
        OccurredOn = DateTime.UtcNow;
    }
} 