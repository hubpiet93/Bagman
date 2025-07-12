using Supabase.Postgrest.Models;

namespace TypowanieMeczy.Infrastructure.Models;

public class SupabaseMatch : BaseModel
{
    public SupabaseMatch() {}
    public string Id { get; set; } = string.Empty;
    public string TableId { get; set; } = string.Empty;
    public string Country1 { get; set; } = string.Empty;
    public string Country2 { get; set; } = string.Empty;
    public DateTime MatchDateTime { get; set; }
    public string? Result { get; set; }
    public string Status { get; set; } = "scheduled";
    public bool Started { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
} 