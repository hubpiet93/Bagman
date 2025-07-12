using TypowanieMeczy.Domain.Entities;

namespace TypowanieMeczy.Api.Models;

public class MatchDto
{
    public string Id { get; set; } = string.Empty;
    public string TableId { get; set; } = string.Empty;
    public string Country1 { get; set; } = string.Empty;
    public string Country2 { get; set; } = string.Empty;
    public DateTime MatchDateTime { get; set; }
    public string? Result { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsStarted { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public PoolDto Pool { get; set; } = new();
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }

    public static MatchDto FromEntity(Match match)
    {
        return new MatchDto
        {
            Id = match.Id.ToString(),
            TableId = match.TableId.ToString(),
            Country1 = match.Country1.Value,
            Country2 = match.Country2.Value,
            MatchDateTime = match.MatchDateTime.Value,
            Result = match.Result?.Value,
            Status = match.Status.ToString(),
            IsStarted = match.IsStarted,
            CreatedBy = match.CreatedBy.ToString(),
            CreatedAt = match.CreatedAt,
            Pool = PoolDto.FromEntity(match.Pool)
        };
    }
}

public class CreateMatchRequest
{
    public string TableId { get; set; } = string.Empty;
    public string Country1 { get; set; } = string.Empty;
    public string Country2 { get; set; } = string.Empty;
    public DateTime MatchDateTime { get; set; }
}

public class UpdateMatchResultRequest
{
    public string Result { get; set; } = string.Empty;
} 