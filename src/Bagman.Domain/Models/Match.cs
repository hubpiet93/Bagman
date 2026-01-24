using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bagman.Domain.Models;

public class Match
{
    public Guid Id { get; set; }

    public Guid TableId { get; set; }

    [Required] [StringLength(100)] public string Country1 { get; set; } = string.Empty;

    [Required] [StringLength(100)] public string Country2 { get; set; } = string.Empty;

    public DateTime MatchDateTime { get; set; }

    [StringLength(10)] public string? Result { get; set; }

    [StringLength(20)] public string Status { get; set; } = "scheduled";

    [NotMapped] public bool Started => DateTime.UtcNow >= MatchDateTime;

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual Table? Table { get; set; }
    public virtual ICollection<Bet> Bets { get; set; } = new List<Bet>();
    public virtual ICollection<Pool> Pools { get; set; } = new List<Pool>();
}
