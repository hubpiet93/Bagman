using Supabase.Postgrest.Models;

namespace TypowanieMeczy.Infrastructure.Models;

public class SupabaseTable : BaseModel
{
    public SupabaseTable() {}
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int MaxPlayers { get; set; }
    public decimal Stake { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsSecretMode { get; set; }
}

public class SupabaseTableMembership : BaseModel
{
    public SupabaseTableMembership() {}
    public string UserId { get; set; } = string.Empty;
    public string TableId { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public DateTime JoinedAt { get; set; }
} 