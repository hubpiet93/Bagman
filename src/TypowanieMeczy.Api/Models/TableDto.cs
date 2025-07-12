using TypowanieMeczy.Domain.Entities;

namespace TypowanieMeczy.Api.Models;

public class TableDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int MaxPlayers { get; set; }
    public decimal Stake { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsSecretMode { get; set; }
    public int MemberCount { get; set; }
    public bool IsAdmin { get; set; }

    public static TableDto FromEntity(Table table)
    {
        return new TableDto
        {
            Id = table.Id.ToString(),
            Name = table.Name.Value,
            MaxPlayers = table.MaxPlayers.Value,
            Stake = table.Stake.Value,
            CreatedBy = table.CreatedBy.ToString(),
            CreatedAt = table.CreatedAt,
            IsSecretMode = table.IsSecretMode,
            MemberCount = table.GetMemberCount()
        };
    }
}

public class CreateTableRequest
{
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int MaxPlayers { get; set; }
    public decimal Stake { get; set; }
    public bool IsSecretMode { get; set; }
}

public class JoinTableRequest
{
    public string Password { get; set; } = string.Empty;
}

public class UpdateTableSettingsRequest
{
    public string Name { get; set; } = string.Empty;
    public int MaxPlayers { get; set; }
    public decimal Stake { get; set; }
    public bool IsSecretMode { get; set; }
}

public class TableMemberDto
{
    public string UserId { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public DateTime JoinedAt { get; set; }

    public static TableMemberDto FromEntity(TableMembership membership)
    {
        return new TableMemberDto
        {
            UserId = membership.UserId.ToString(),
            IsAdmin = membership.IsAdmin,
            JoinedAt = membership.JoinedAt
        };
    }
} 