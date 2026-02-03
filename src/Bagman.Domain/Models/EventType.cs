using System.ComponentModel.DataAnnotations;
using Bagman.Domain.Common;
using ErrorOr;

namespace Bagman.Domain.Models;

public class EventType
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();
    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    // Domain methods
    public ErrorOr<Success> Deactivate()
    {
        if (!IsActive)
        {
            return Error.Validation("EventType.AlreadyDeactivated", "Typ wydarzenia jest już nieaktywny");
        }

        IsActive = false;
        return Result.Success;
    }

    public ErrorOr<Success>  Activate()
    {
        if (IsActive)
        {
            return Error.Validation("EventType.AlreadyActive", "Typ wydarzenia jest już aktywny");
        }

        IsActive = true;
        return Result.Success;
    }
}
