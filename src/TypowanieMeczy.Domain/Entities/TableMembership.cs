using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Entities;

public class TableMembership
{
    public UserId UserId { get; private set; }
    public TableId TableId { get; private set; }
    public bool IsAdmin { get; private set; }
    public DateTime JoinedAt { get; private set; }

    private TableMembership() { } // For EF Core

    public TableMembership(UserId userId, TableId tableId, bool isAdmin = false)
    {
        UserId = userId;
        TableId = tableId;
        IsAdmin = isAdmin;
        JoinedAt = DateTime.UtcNow;
    }

    public void GrantAdminRole()
    {
        IsAdmin = true;
    }

    public void RevokeAdminRole()
    {
        IsAdmin = false;
    }
} 