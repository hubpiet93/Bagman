namespace Bagman.Domain.Models;

public class TableMember
{
    public Guid UserId { get; set; }

    public Guid TableId { get; set; }

    public bool IsAdmin { get; set; }

    public DateTime JoinedAt { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual Table? Table { get; set; }
}
