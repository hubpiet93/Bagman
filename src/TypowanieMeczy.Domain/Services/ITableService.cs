using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Services;

public interface ITableService
{
    Task<Table> CreateTableAsync(TableName name, PasswordHash passwordHash, MaxPlayers maxPlayers, Stake stake, UserId createdBy);
    Task<Table> JoinTableAsync(TableName tableName, PasswordHash tablePassword, UserId userId);
    Task LeaveTableAsync(TableId tableId, UserId userId);
    Task GrantAdminRoleAsync(TableId tableId, UserId targetUserId, UserId adminUserId);
    Task RevokeAdminRoleAsync(TableId tableId, UserId targetUserId, UserId adminUserId);
    Task UpdateTableSettingsAsync(TableId tableId, TableName name, MaxPlayers maxPlayers, Stake stake, bool isSecretMode, UserId adminUserId);
    Task<IEnumerable<Table>> GetUserTablesAsync(UserId userId);
    Task<Table> GetTableDetailsAsync(TableId tableId, UserId userId);
    Task<bool> IsUserMemberAsync(TableId tableId, UserId userId);
    Task<bool> IsUserAdminAsync(TableId tableId, UserId userId);
    Task<IEnumerable<TableMembership>> GetTableMembersAsync(TableId tableId, UserId userId);
    Task<bool> CanUserJoinTableAsync(TableId tableId, UserId userId);
    Task<bool> CanUserLeaveTableAsync(TableId tableId, UserId userId);
    Task<bool> CanUserDeleteTableAsync(TableId tableId, UserId userId);
    Task<bool> CanUserEditTableAsync(TableId tableId, UserId userId);
    Task<bool> IsTableFullAsync(TableId tableId);
    Task<bool> IsTableSecretAsync(TableId tableId);
    Task<bool> ValidateTablePasswordAsync(TableId tableId, string password);
} 