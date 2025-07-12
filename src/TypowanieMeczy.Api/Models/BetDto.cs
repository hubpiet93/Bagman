using TypowanieMeczy.Domain.Entities;

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

    public static BetDto FromEntity(Bet bet)
    {
        return new BetDto
        {
            Id = bet.Id.ToString(),
            UserId = bet.UserId.ToString(),
            MatchId = bet.MatchId.ToString(),
            Prediction = bet.Prediction.Value,
            EditedAt = bet.EditedAt,
            IsWinner = bet.IsWinner
        };
    }
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