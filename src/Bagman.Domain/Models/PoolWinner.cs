using System.ComponentModel.DataAnnotations;

namespace Bagman.Domain.Models;

public class PoolWinner
{
    public Guid PoolId { get; set; }

    public Guid UserId { get; set; }

    [Range(0, double.MaxValue)] public decimal AmountWon { get; set; }

    // Navigation properties
    public virtual Pool? Pool { get; set; }
    public virtual User? User { get; set; }
}
