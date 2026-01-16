using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bagman.Infrastructure.Models;

[Table("users")]
public class UserEntity
{
    [Key] [Column("id")] public Guid Id { get; set; }

    [Required] [Column("login")] public string Login { get; set; } = string.Empty;

    [Required] [Column("email")] public string Email { get; set; } = string.Empty;

    [Column("created_at")] public DateTime CreatedAt { get; set; }

    [Column("is_active")] public bool IsActive { get; set; } = true;
}
