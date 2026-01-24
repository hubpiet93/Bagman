using System.ComponentModel.DataAnnotations;

namespace Bagman.Domain.Models;

public class Pool
{
    public Guid Id { get; set; }

    public Guid MatchId { get; set; }

    [Range(0, double.MaxValue)] public decimal Amount { get; set; }

    [StringLength(20)] public string Status { get; set; } = "active"; // active, won, rollover, expired

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual Match? Match { get; set; }
    public virtual ICollection<PoolWinner> Winners { get; set; } = new List<PoolWinner>();
}
