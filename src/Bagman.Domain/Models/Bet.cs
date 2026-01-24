using System.ComponentModel.DataAnnotations;

namespace Bagman.Domain.Models;

public class Bet
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid MatchId { get; set; }

    [Required] [StringLength(10)] public string Prediction { get; set; } = string.Empty;

    public DateTime EditedAt { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual Match? Match { get; set; }
}
