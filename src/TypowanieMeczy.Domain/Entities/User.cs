using TypowanieMeczy.Domain.Common;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Domain.Events;
using TypowanieMeczy.Domain.Exceptions;

namespace TypowanieMeczy.Domain.Entities;

public class User : AggregateRoot
{
    public UserId Id { get; private set; }
    public Login Login { get; private set; }
    public Email Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<TableMembership> _tableMemberships = new();
    public IReadOnlyCollection<TableMembership> TableMemberships => _tableMemberships.AsReadOnly();

    private User() { } // For EF Core

    public User(Login login, Email email, PasswordHash passwordHash)
    {
        Id = UserId.New();
        Login = login;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;

        AddDomainEvent(new UserCreatedEvent(Id, Login, Email));
    }

    public void JoinTable(Table table, bool isAdmin = false)
    {
        if (_tableMemberships.Any(tm => tm.TableId == table.Id))
        {
            throw new DomainException("User is already a member of this table");
        }

        var membership = new TableMembership(Id, table.Id, isAdmin);
        _tableMemberships.Add(membership);

        AddDomainEvent(new UserJoinedTableEvent(Id, table.Id));
    }

    public void LeaveTable(TableId tableId)
    {
        var membership = _tableMemberships.FirstOrDefault(tm => tm.TableId == tableId);
        if (membership == null)
        {
            throw new DomainException("User is not a member of this table");
        }

        _tableMemberships.Remove(membership);
        AddDomainEvent(new UserLeftTableEvent(Id, tableId));
    }

    public bool IsAdminOfTable(TableId tableId)
    {
        return _tableMemberships.Any(tm => tm.TableId == tableId && tm.IsAdmin);
    }

    public bool IsMemberOfTable(TableId tableId)
    {
        return _tableMemberships.Any(tm => tm.TableId == tableId);
    }

    public void Deactivate()
    {
        IsActive = false;
        AddDomainEvent(new UserDeactivatedEvent(Id));
    }

    public void Activate()
    {
        IsActive = true;
        AddDomainEvent(new UserActivatedEvent(Id));
    }
} 