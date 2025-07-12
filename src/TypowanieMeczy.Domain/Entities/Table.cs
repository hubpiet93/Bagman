using TypowanieMeczy.Domain.Common;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Domain.Events;
using TypowanieMeczy.Domain.Exceptions;

namespace TypowanieMeczy.Domain.Entities;

public class Table : AggregateRoot
{
    public TableId Id { get; private set; }
    public TableName Name { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public MaxPlayers MaxPlayers { get; private set; }
    public Stake Stake { get; private set; }
    public UserId CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsSecretMode { get; private set; }

    private readonly List<TableMembership> _memberships = new();
    public IReadOnlyCollection<TableMembership> Memberships => _memberships.AsReadOnly();

    private readonly List<Match> _matches = new();
    public IReadOnlyCollection<Match> Matches => _matches.AsReadOnly();

    private Table() { } // For EF Core

    public Table(TableName name, PasswordHash passwordHash, MaxPlayers maxPlayers, Stake stake, UserId createdBy)
    {
        Id = TableId.New();
        Name = name;
        PasswordHash = passwordHash;
        MaxPlayers = maxPlayers;
        Stake = stake;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        IsSecretMode = false;

        // Creator becomes first admin
        var creatorMembership = new TableMembership(createdBy, Id, true);
        _memberships.Add(creatorMembership);

        AddDomainEvent(new TableCreatedEvent(Id, Name, CreatedBy));
    }

    public void AddMember(UserId userId, bool isAdmin = false)
    {
        if (_memberships.Count >= MaxPlayers.Value)
        {
            throw new TableFullException($"Table {Name.Value} is full. Maximum players: {MaxPlayers.Value}");
        }

        if (_memberships.Any(m => m.UserId == userId))
        {
            throw new DomainException("User is already a member of this table");
        }

        var membership = new TableMembership(userId, Id, isAdmin);
        _memberships.Add(membership);

        AddDomainEvent(new MemberAddedToTableEvent(userId, Id));
    }

    public void RemoveMember(UserId userId)
    {
        var membership = _memberships.FirstOrDefault(m => m.UserId == userId);
        if (membership == null)
        {
            throw new DomainException("User is not a member of this table");
        }

        // Check if this would leave the table without admins
        if (membership.IsAdmin && GetAdminCount() <= 1)
        {
            throw new DomainException("Cannot remove the last admin from the table");
        }

        _memberships.Remove(membership);
        AddDomainEvent(new MemberRemovedFromTableEvent(userId, Id));
    }

    public void GrantAdminRole(UserId userId)
    {
        var membership = _memberships.FirstOrDefault(m => m.UserId == userId);
        if (membership == null)
        {
            throw new DomainException("User is not a member of this table");
        }

        if (membership.IsAdmin)
        {
            throw new DomainException("User is already an admin");
        }

        membership.GrantAdminRole();
        AddDomainEvent(new AdminRoleGrantedEvent(userId, Id));
    }

    public void RevokeAdminRole(UserId userId)
    {
        var membership = _memberships.FirstOrDefault(m => m.UserId == userId);
        if (membership == null)
        {
            throw new DomainException("User is not a member of this table");
        }

        if (!membership.IsAdmin)
        {
            throw new DomainException("User is not an admin");
        }

        if (GetAdminCount() <= 1)
        {
            throw new DomainException("Cannot revoke admin role from the last admin");
        }

        membership.RevokeAdminRole();
        AddDomainEvent(new AdminRoleRevokedEvent(userId, Id));
    }

    public void AddMatch(Match match)
    {
        if (!IsAdmin(match.CreatedBy))
        {
            throw new UnauthorizedAccessException("Only admins can add matches");
        }

        _matches.Add(match);
        AddDomainEvent(new MatchAddedEvent(match.Id, Id));
    }

    public void UpdateSettings(TableName name, MaxPlayers maxPlayers, Stake stake, bool isSecretMode)
    {
        if (maxPlayers.Value < _memberships.Count)
        {
            throw new DomainException($"Cannot reduce max players below current member count ({_memberships.Count})");
        }

        Name = name;
        MaxPlayers = maxPlayers;
        Stake = stake;
        IsSecretMode = isSecretMode;

        AddDomainEvent(new TableSettingsUpdatedEvent(Id));
    }

    public void UpdateName(TableName name)
    {
        Name = name;
        AddDomainEvent(new TableSettingsUpdatedEvent(Id));
    }
    public void UpdateMaxPlayers(MaxPlayers maxPlayers)
    {
        MaxPlayers = maxPlayers;
        AddDomainEvent(new TableSettingsUpdatedEvent(Id));
    }
    public void UpdateStake(Stake stake)
    {
        Stake = stake;
        AddDomainEvent(new TableSettingsUpdatedEvent(Id));
    }

    public bool IsAdmin(UserId userId)
    {
        return _memberships.Any(m => m.UserId == userId && m.IsAdmin);
    }

    public bool IsMember(UserId userId)
    {
        return _memberships.Any(m => m.UserId == userId);
    }

    public int GetMemberCount()
    {
        return _memberships.Count;
    }

    private int GetAdminCount()
    {
        return _memberships.Count(m => m.IsAdmin);
    }

    public decimal CalculatePoolForMatch(MatchId matchId)
    {
        var match = _matches.FirstOrDefault(m => m.Id == matchId);
        if (match == null)
        {
            throw new DomainException("Match not found");
        }

        var basePool = _memberships.Count * Stake.Value;
        
        // Add rollover from previous matches
        var previousMatches = _matches.Where(m => m.Id != matchId && m.IsFinished && !m.HasWinners).ToList();
        var rolloverAmount = previousMatches.Sum(m => m.Pool.Amount);

        return basePool + rolloverAmount;
    }
} 