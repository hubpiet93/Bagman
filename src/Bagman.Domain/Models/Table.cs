using System.ComponentModel.DataAnnotations;

namespace Bagman.Domain.Models;

public class Table
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int MaxPlayers { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Stake { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsSecretMode { get; set; }

    // Navigation properties
    public virtual User? CreatedByUser { get; set; }
    public virtual ICollection<TableMember> Members { get; set; } = new List<TableMember>();
    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
    public virtual ICollection<UserStats> UserStats { get; set; } = new List<UserStats>();
}
