using Supabase.Postgrest.Models;

namespace TypowanieMeczy.Infrastructure.Models;

public class SupabasePool : BaseModel
{
    public SupabasePool() {}
    public string Id { get; set; } = string.Empty;
    public string MatchId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = "active";
    public DateTime CreatedAt { get; set; }
} 