using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Entities;

public class Pool
{
    public PoolId Id { get; private set; }
    public MatchId MatchId { get; private set; }
    public decimal Amount { get; private set; }
    public PoolStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<UserId> _winners = new();
    public IReadOnlyCollection<UserId> Winners => _winners.AsReadOnly();

    private Pool() { } // For EF Core

    public Pool(MatchId matchId)
    {
        Id = PoolId.New();
        MatchId = matchId;
        Amount = 0;
        Status = PoolStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetAmount(decimal amount)
    {
        Amount = amount;
    }

    public void DistributeToWinners(List<UserId> winners, decimal amountPerWinner)
    {
        _winners.Clear();
        _winners.AddRange(winners);
        Status = PoolStatus.Won;
    }

    public void MarkForRollover()
    {
        Status = PoolStatus.Rollover;
    }

    public void MarkAsExpired()
    {
        Status = PoolStatus.Expired;
    }
}

public enum PoolStatus
{
    Active,
    Won,
    Rollover,
    Expired
} 