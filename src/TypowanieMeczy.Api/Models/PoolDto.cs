using TypowanieMeczy.Domain.Entities;
using System.Linq;

namespace TypowanieMeczy.Api.Models;

public class PoolDto
{
    public string Id { get; set; } = string.Empty;
    public string MatchId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<string> Winners { get; set; } = new();

    public static PoolDto FromEntity(Pool pool)
    {
        return new PoolDto
        {
            Id = pool.Id.ToString(),
            MatchId = pool.MatchId.ToString(),
            Amount = pool.Amount,
            Status = pool.Status.ToString(),
            CreatedAt = pool.CreatedAt,
            Winners = pool.Winners.Select(w => w.ToString()).ToList()
        };
    }
}

public enum PoolStatusDto
{
    Active,
    Won,
    Rollover,
    Expired
} 