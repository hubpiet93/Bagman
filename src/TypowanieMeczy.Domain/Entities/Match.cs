using TypowanieMeczy.Domain.Common;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Domain.Events;
using TypowanieMeczy.Domain.Exceptions;

namespace TypowanieMeczy.Domain.Entities;

public class Match : AggregateRoot
{
    public MatchId Id { get; private set; }
    public TableId TableId { get; private set; }
    public Country Country1 { get; private set; }
    public Country Country2 { get; private set; }
    public MatchDateTime MatchDateTime { get; private set; }
    public MatchResult? Result { get; private set; }
    public MatchStatus Status { get; private set; }
    public bool IsStarted { get; private set; }
    public UserId CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<Bet> _bets = new();
    public IReadOnlyCollection<Bet> Bets => _bets.AsReadOnly();

    public Pool Pool { get; private set; }

    private Match() { } // For EF Core

    public Match(TableId tableId, Country country1, Country country2, MatchDateTime matchDateTime, UserId createdBy)
    {
        Id = MatchId.New();
        TableId = tableId;
        Country1 = country1;
        Country2 = country2;
        MatchDateTime = matchDateTime;
        Status = MatchStatus.Scheduled;
        IsStarted = false;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;

        Pool = new Pool(Id);

        AddDomainEvent(new MatchCreatedEvent(Id, TableId, Country1, Country2, MatchDateTime));
    }

    public void PlaceBet(UserId userId, MatchPrediction prediction)
    {
        if (IsStarted)
        {
            throw new MatchAlreadyStartedException("Cannot place bet after match has started");
        }

        var existingBet = _bets.FirstOrDefault(b => b.UserId == userId);
        if (existingBet != null)
        {
            existingBet.UpdatePrediction(prediction);
            AddDomainEvent(new BetUpdatedEvent(Id, userId, prediction));
        }
        else
        {
            var bet = new Bet(userId, Id, prediction);
            _bets.Add(bet);
            AddDomainEvent(new BetPlacedEvent(Id, userId, prediction));
        }
    }

    public void StartMatch()
    {
        if (IsStarted)
        {
            throw new DomainException("Match is already started");
        }

        if (DateTime.UtcNow < MatchDateTime.Value)
        {
            throw new DomainException("Cannot start match before scheduled time");
        }

        IsStarted = true;
        Status = MatchStatus.InProgress;
        AddDomainEvent(new MatchStartedEvent(Id));
    }

    public void FinishMatch(MatchResult result)
    {
        if (!IsStarted)
        {
            throw new DomainException("Match must be started before it can be finished");
        }

        if (Status == MatchStatus.Finished)
        {
            throw new DomainException("Match is already finished");
        }

        Result = result;
        Status = MatchStatus.Finished;
        IsStarted = false;

        // Calculate winners and distribute pool
        CalculateWinners();

        AddDomainEvent(new MatchFinishedEvent(Id, result));
    }

    public void UpdateResult(MatchResult result)
    {
        if (Status != MatchStatus.Finished)
        {
            throw new DomainException("Can only update result for finished matches");
        }

        Result = result;
        
        // Recalculate winners
        CalculateWinners();

        AddDomainEvent(new MatchResultUpdatedEvent(Id, result));
    }

    private void CalculateWinners()
    {
        if (Result == null || Status != MatchStatus.Finished)
        {
            return;
        }

        var winners = _bets.Where(b => b.Prediction.Value == Result.Value).ToList();
        
        if (winners.Any())
        {
            var amountPerWinner = Pool.Amount / winners.Count;
            Pool.DistributeToWinners(winners.Select(w => w.UserId).ToList(), amountPerWinner);
            
            foreach (var winner in winners)
            {
                winner.MarkAsWinner();
            }

            AddDomainEvent(new PoolWonEvent(Id, winners.Select(w => w.UserId).ToList(), Pool.Amount));
        }
        else
        {
            // No winners - pool rolls over
            Pool.MarkForRollover();
            AddDomainEvent(new PoolRolloverEvent(Id, Pool.Amount));
        }
    }

    public bool HasWinners => Pool.Winners.Any();

    public IReadOnlyCollection<Bet> GetBetsForUser(UserId userId)
    {
        return _bets.Where(b => b.UserId == userId).ToList().AsReadOnly();
    }

    public Bet? GetBetForUser(UserId userId)
    {
        return _bets.FirstOrDefault(b => b.UserId == userId);
    }

    public bool HasUserPlacedBet(UserId userId)
    {
        return _bets.Any(b => b.UserId == userId);
    }

    public void SetPoolAmount(decimal amount)
    {
        Pool.SetAmount(amount);
    }

    public bool CanBeEditedBy(UserId userId)
    {
        return CreatedBy == userId && !IsStarted;
    }

    public bool CanBeDeletedBy(UserId userId)
    {
        return CreatedBy == userId && !IsStarted;
    }

    public bool IsFinished => Status == MatchStatus.Finished;
} 