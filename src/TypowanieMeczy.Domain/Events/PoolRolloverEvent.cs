using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Events;

public class PoolRolloverEvent : IDomainEvent
{
    public MatchId MatchId { get; }
    public decimal Amount { get; }
    public DateTime OccurredOn { get; }

    public PoolRolloverEvent(MatchId matchId, decimal amount)
    {
        MatchId = matchId;
        Amount = amount;
        OccurredOn = DateTime.UtcNow;
    }
} 