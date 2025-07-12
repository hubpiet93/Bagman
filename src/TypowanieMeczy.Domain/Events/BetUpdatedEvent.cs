using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Events;

public class BetUpdatedEvent : IDomainEvent
{
    public MatchId MatchId { get; }
    public UserId UserId { get; }
    public MatchPrediction Prediction { get; }
    public DateTime OccurredOn { get; }

    public BetUpdatedEvent(MatchId matchId, UserId userId, MatchPrediction prediction)
    {
        MatchId = matchId;
        UserId = userId;
        Prediction = prediction;
        OccurredOn = DateTime.UtcNow;
    }
} 