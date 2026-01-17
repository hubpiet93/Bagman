using System.ComponentModel.DataAnnotations;

namespace Bagman.Domain.Models;

public class UserStats
{
    public Guid UserId { get; set; }

    public Guid TableId { get; set; }

    public int MatchesPlayed { get; set; }

    public int BetsPlaced { get; set; }

    public int PoolsWon { get; set; }

    [Range(0, double.MaxValue)]
    public decimal TotalWon { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual Table? Table { get; set; }
}
