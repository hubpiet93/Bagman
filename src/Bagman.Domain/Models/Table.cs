using Bagman.Domain.Common.ValueObjects;
using ErrorOr;

namespace Bagman.Domain.Models;

/// <summary>
///     Table aggregate root - represents a betting table for a group of users
/// </summary>
public class Table
{
    private readonly List<TableMember> _members = new();

    public Guid Id { get; }
    public TableName Name { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public int MaxPlayers { get; }
    public Money Stake { get; private set; } = null!;
    public Guid CreatedBy { get; private set; }
    public Guid EventTypeId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsSecretMode { get; private set; }

    // Navigation properties
    public virtual User? CreatedByUser { get; private set; }
    public virtual EventType? EventType { get; private set; }
    public virtual IReadOnlyCollection<TableMember> Members => _members.AsReadOnly();
    public virtual ICollection<UserStats> UserStats { get; private set; } = new List<UserStats>();

    // EF Core constructor
    private Table()
    {
    }

    private Table(
        Guid id,
        TableName name,
        string passwordHash,
        int maxPlayers,
        Money stake,
        Guid createdBy,
        Guid eventTypeId,
        bool isSecretMode)
    {
        Id = id;
        Name = name;
        PasswordHash = passwordHash;
        MaxPlayers = maxPlayers;
        Stake = stake;
        CreatedBy = createdBy;
        EventTypeId = eventTypeId;
        CreatedAt = DateTime.UtcNow;
        IsSecretMode = isSecretMode;
    }

    public static ErrorOr<Table> Create(
        TableName name,
        string passwordHash,
        int maxPlayers,
        Money stake,
        Guid createdBy,
        Guid eventTypeId,
        bool isSecretMode = false)
    {
        if (maxPlayers < 1)
            return Error.Validation(
                "Table.InvalidMaxPlayers",
                "Max players must be at least 1");

        var table = new Table(
            Guid.NewGuid(),
            name,
            passwordHash,
            maxPlayers,
            stake,
            createdBy,
            eventTypeId,
            isSecretMode);

        // Add creator as admin member
        var member = new TableMember
        {
            UserId = createdBy,
            TableId = table.Id,
            IsAdmin = true,
            JoinedAt = DateTime.UtcNow
        };

        table._members.Add(member);

        return table;
    }

    public bool IsFull()
    {
        return _members.Count >= MaxPlayers;
    }

    public bool IsUserMember(Guid userId)
    {
        return _members.Any(m => m.UserId == userId);
    }

    public bool IsUserAdmin(Guid userId)
    {
        return _members.Any(m => m.UserId == userId && m.IsAdmin);
    }

    public ErrorOr<Success> AddMember(Guid userId, string providedPassword)
    {
        if (IsUserMember(userId))
            return Error.Conflict(
                "Table.AlreadyMember",
                "Użytkownik jest już członkiem tego stołu");

        if (IsFull())
            return Error.Validation(
                "Table.Full",
                "Stół jest pełny");

        // Note: Password verification should be done externally before calling this method
        // as it requires infrastructure (password hasher)

        var member = new TableMember
        {
            UserId = userId,
            TableId = Id,
            IsAdmin = false,
            JoinedAt = DateTime.UtcNow
        };

        _members.Add(member);

        return Result.Success;
    }

    public ErrorOr<Success> RemoveMember(Guid userId)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member == null)
            return Error.NotFound(
                "Table.MemberNotFound",
                "Członek nie został znaleziony");

        _members.Remove(member);
        return Result.Success;
    }

    public ErrorOr<Success> GrantAdmin(Guid requestingUserId, Guid targetUserId)
    {
        if (!IsUserAdmin(requestingUserId))
            return Error.Forbidden(
                "Table.NotAdmin",
                "Nie masz uprawnień do zarządzania administratorami");

        var member = _members.FirstOrDefault(m => m.UserId == targetUserId);
        if (member == null)
            return Error.NotFound(
                "Table.MemberNotFound",
                "Członek nie został znaleziony");

        member.IsAdmin = true;
        return Result.Success;
    }

    public ErrorOr<Success> RevokeAdmin(Guid requestingUserId, Guid targetUserId)
    {
        if (!IsUserAdmin(requestingUserId))
            return Error.Forbidden(
                "Table.NotAdmin",
                "Nie masz uprawnień do zarządzania administratorami");

        var member = _members.FirstOrDefault(m => m.UserId == targetUserId);
        if (member == null)
            return Error.NotFound(
                "Table.MemberNotFound",
                "Członek nie został znaleziony");

        member.IsAdmin = false;
        return Result.Success;
    }
}
