using Supabase.Postgrest.Models;

namespace TypowanieMeczy.Infrastructure.Models;

public class SupabaseUser : BaseModel
{
    public SupabaseUser() {}
    public string Id { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
} 