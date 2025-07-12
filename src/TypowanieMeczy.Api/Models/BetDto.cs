namespace TypowanieMeczy.Api.Models;

public class BetDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string MatchId { get; set; } = string.Empty;
    public string Prediction { get; set; } = string.Empty;
    public DateTime EditedAt { get; set; }
    public bool IsWinner { get; set; }
    public string UserName { get; set; } = string.Empty;
}

public class CreateBetRequest
{
    public string MatchId { get; set; } = string.Empty;
    public string Prediction { get; set; } = string.Empty;
}

public class UpdateBetRequest
{
    public string Prediction { get; set; } = string.Empty;
} 