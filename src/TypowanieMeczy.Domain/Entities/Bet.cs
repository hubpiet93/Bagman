using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Entities;

public class Bet
{
    public BetId Id { get; private set; }
    public UserId UserId { get; private set; }
    public MatchId MatchId { get; private set; }
    public MatchPrediction Prediction { get; private set; }
    public DateTime EditedAt { get; private set; }
    public bool IsWinner { get; private set; }

    private Bet() { } // For EF Core

    public Bet(UserId userId, MatchId matchId, MatchPrediction prediction)
    {
        Id = BetId.New();
        UserId = userId;
        MatchId = matchId;
        Prediction = prediction;
        EditedAt = DateTime.UtcNow;
        IsWinner = false;
    }

    public void UpdatePrediction(MatchPrediction prediction)
    {
        Prediction = prediction;
        EditedAt = DateTime.UtcNow;
    }

    public void MarkAsWinner()
    {
        IsWinner = true;
    }
} 