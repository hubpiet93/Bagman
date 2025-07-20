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
}
