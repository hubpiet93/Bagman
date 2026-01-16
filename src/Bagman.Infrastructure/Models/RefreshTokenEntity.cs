using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bagman.Infrastructure.Models;

[Table("refresh_tokens")]
public class RefreshTokenEntity
{
    [Key]
    [Column("token")]
    [StringLength(512)]
    public string Token { get; set; } = string.Empty;

    [Required] [Column("user_id")] public Guid UserId { get; set; }

    [Column("expires_at")] public DateTime ExpiresAt { get; set; }
}
