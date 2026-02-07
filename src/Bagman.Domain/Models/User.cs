using System.ComponentModel.DataAnnotations;

namespace Bagman.Domain.Models;

public class User
{
    public Guid Id { get; set; }

    [Required] [StringLength(50)] public string Login { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsSuperAdmin { get; set; } = false;

    // Stored password hash (PBKDF2). Nullable to allow external users without password (e.g., OAuth).
    public string? PasswordHash { get; set; }

    // Navigation properties
    // Note: TableMemberships and Bets are owned by Table and Match aggregates, so no direct navigation
    public virtual ICollection<Table> CreatedTables { get; set; } = new List<Table>();
    public virtual ICollection<UserStats> Stats { get; set; } = new List<UserStats>();
    public virtual ICollection<PoolWinner> PoolWinnings { get; set; } = new List<PoolWinner>();
}
