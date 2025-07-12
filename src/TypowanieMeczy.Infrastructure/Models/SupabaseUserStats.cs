using Supabase.Postgrest.Models;

namespace TypowanieMeczy.Infrastructure.Models;

public class SupabaseUserStats : BaseModel
{
    public SupabaseUserStats() {}
    public string UserId { get; set; } = string.Empty;
    public string TableId { get; set; } = string.Empty;
    public int MatchesPlayed { get; set; }
    public int BetsPlaced { get; set; }
    public int PoolsWon { get; set; }
    public decimal TotalWon { get; set; }
    public DateTime UpdatedAt { get; set; }
} 