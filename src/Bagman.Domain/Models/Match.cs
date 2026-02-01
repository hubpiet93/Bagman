using Bagman.Domain.Common.ValueObjects;
using ErrorOr;

namespace Bagman.Domain.Models;

/// <summary>
/// Match aggregate root - represents a football match with bets
/// </summary>
public class Match
{
    private readonly List<Bet> _bets = new();

    // EF Core constructor
    private Match()
    {
    }

    private Match(
        Guid id,
        Guid tableId,
        Country country1,
        Country country2,
        DateTime matchDateTime,
        string status)
    {
        Id = id;
        TableId = tableId;
        Country1 = country1;
        Country2 = country2;
        MatchDateTime = matchDateTime;
        Status = status;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TableId { get; private set; }
    public Country Country1 { get; private set; } = null!;
    public Country Country2 { get; private set; } = null!;
    public DateTime MatchDateTime { get; private set; }
    public Score? Result { get; private set; }
    public string Status { get; private set; } = "scheduled";
    public bool Started => DateTime.UtcNow >= MatchDateTime;
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public virtual Table? Table { get; private set; }
    public virtual IReadOnlyCollection<Bet> Bets => _bets.AsReadOnly();
    public virtual ICollection<Pool> Pools { get; private set; } = new List<Pool>();

    public static ErrorOr<Match> Create(
        Guid tableId,
        Country country1,
        Country country2,
        DateTime matchDateTime)
    {
        if (matchDateTime <= DateTime.UtcNow)
        {
            return Error.Validation(
                "Match.InvalidDateTime",
                "Match date must be in the future");
        }

        return new Match(
            Guid.NewGuid(),
            tableId,
            country1,
            country2,
            matchDateTime,
            "scheduled");
    }

    public ErrorOr<Success> Update(Country country1, Country country2, DateTime matchDateTime)
    {
        if (Started)
        {
            return Error.Failure(
                "Match.AlreadyStarted",
                "Nie można edytować meczu, który już się rozpoczął");
        }

        Country1 = country1;
        Country2 = country2;
        MatchDateTime = matchDateTime;

        return ErrorOr.Result.Success;
    }

    public ErrorOr<Success> SetResult(Score result)
    {
        Result = result;
        Status = "finished";
        return ErrorOr.Result.Success;
    }

    public ErrorOr<Success> PlaceBet(Guid userId, Prediction prediction)
    {
        if (Started)
        {
            return Error.Failure(
                "Match.AlreadyStarted",
                "Nie można typować na mecz, który już się rozpoczął");
        }

        var existingBet = _bets.FirstOrDefault(b => b.UserId == userId);
        if (existingBet != null)
        {
            existingBet.UpdatePrediction(prediction);
            return ErrorOr.Result.Success;
        }

        var bet = new Bet
        {
            // REMOVED: Id = Guid.NewGuid(), - Let EF Core generate it as ValueGeneratedOnAdd
            UserId = userId,
            MatchId = Id,
            Prediction = prediction,
            EditedAt = DateTime.UtcNow
        };

        _bets.Add(bet);
        return ErrorOr.Result.Success;
    }

    public ErrorOr<Success> RemoveBet(Guid userId)
    {
        if (Started)
        {
            return Error.Failure(
                "Match.AlreadyStarted",
                "Nie można usunąć typu na mecz, który już się rozpoczął");
        }

        var bet = _bets.FirstOrDefault(b => b.UserId == userId);
        if (bet == null)
        {
            return Error.NotFound(
                "Bet.NotFound",
                "Typ nie został znaleziony");
        }

        _bets.Remove(bet);
        return ErrorOr.Result.Success;
    }

    public Bet? GetUserBet(Guid userId)
    {
        return _bets.FirstOrDefault(b => b.UserId == userId);
    }

    public ErrorOr<Success> Delete()
    {
        if (Started)
        {
            return Error.Failure(
                "Match.AlreadyStarted",
                "Nie można usunąć meczu, który już się rozpoczął");
        }

        return ErrorOr.Result.Success;
    }
}
