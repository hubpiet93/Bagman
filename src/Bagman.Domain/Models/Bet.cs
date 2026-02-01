using Bagman.Domain.Common.ValueObjects;

namespace Bagman.Domain.Models;

/// <summary>
/// Bet entity - owned by Match aggregate
/// </summary>
public class Bet
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MatchId { get; set; }
    public Prediction Prediction { get; set; } = null!;
    public DateTime EditedAt { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual Match? Match { get; set; }

    internal void UpdatePrediction(Prediction newPrediction)
    {
        Prediction = newPrediction;
        EditedAt = DateTime.UtcNow;
    }
}
