using Supabase.Postgrest.Models;

namespace TypowanieMeczy.Infrastructure.Models;

public class SupabaseBet : BaseModel
{
    public SupabaseBet() {}
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string MatchId { get; set; } = string.Empty;
    public string Prediction { get; set; } = string.Empty;
    public DateTime EditedAt { get; set; }
    public bool IsWinner { get; set; }
} 