using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Events;

public class PoolWonEvent : IDomainEvent
{
    public MatchId MatchId { get; }
    public List<UserId> Winners { get; }
    public decimal Amount { get; }
    public DateTime OccurredOn { get; }

    public PoolWonEvent(MatchId matchId, List<UserId> winners, decimal amount)
    {
        MatchId = matchId;
        Winners = winners;
        Amount = amount;
        OccurredOn = DateTime.UtcNow;
    }
} 