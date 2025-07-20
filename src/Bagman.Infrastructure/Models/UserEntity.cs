using Postgrest.Attributes;
using Postgrest.Models;

namespace Bagman.Infrastructure.Models;

[Table("users")]
public class UserEntity : BaseModel
{
    [PrimaryKey("id")] public Guid Id { get; set; }

    [Column("login")] public string Login { get; set; } = string.Empty;

    [Column("email")] public string Email { get; set; } = string.Empty;

    [Column("created_at")] public DateTime CreatedAt { get; set; }

    [Column("is_active")] public bool IsActive { get; set; } = true;
}
