namespace Bagman.Contracts.Models.Tables;

public record CreateTableRequest
{
    public required string UserLogin { get; init; }
    public required string UserPassword { get; init; }
    public required string TableName { get; init; }
    public required string TablePassword { get; init; }
    public required int MaxPlayers { get; init; }
    public required decimal Stake { get; init; }
    public required Guid EventTypeId { get; init; }
}

public record AuthorizedCreateTableRequest
{
    public required string TableName { get; init; }
    public required string TablePassword { get; init; }
    public required int MaxPlayers { get; init; }
    public required decimal Stake { get; init; }
    public required Guid EventTypeId { get; init; }
}

public record JoinTableRequest
{
    public required string UserLogin { get; init; }
    public required string UserPassword { get; init; }
    public required string TableName { get; init; }
    public required string TablePassword { get; init; }
}

public record JoinTableAuthorizedRequest
{
    public required string Password { get; init; }
}

public record TableResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int MaxPlayers { get; init; }
    public required decimal Stake { get; init; }
    public required Guid CreatedBy { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required bool IsSecretMode { get; init; }
}

public record TableDetailResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int MaxPlayers { get; init; }
    public required decimal Stake { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required List<TableMemberResponse> Members { get; init; }
}

public record TableMemberResponse
{
    public required Guid UserId { get; init; }
    public required string Login { get; init; }
    public required bool IsAdmin { get; init; }
    public required DateTime JoinedAt { get; init; }
}

public record JoinTableResponse
{
    // Table information
    public required Guid TableId { get; init; }
    public required string TableName { get; init; }
    public required int MaxPlayers { get; init; }
    public required decimal Stake { get; init; }
    public required DateTime TableCreatedAt { get; init; }

    // Member information (the joining user)
    public required Guid UserId { get; init; }
    public required string UserLogin { get; init; }
    public required bool IsAdmin { get; init; }
    public required DateTime JoinedAt { get; init; }

    // Table status
    public required int CurrentMemberCount { get; init; }
}
