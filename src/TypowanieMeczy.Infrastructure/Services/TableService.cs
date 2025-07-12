using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.Services;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Domain.Interfaces;

namespace TypowanieMeczy.Infrastructure.Services;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;
    private readonly IUserRepository _userRepository;

    public TableService(ITableRepository tableRepository, IUserRepository userRepository)
    {
        _tableRepository = tableRepository;
        _userRepository = userRepository;
    }

    public async Task<Table> CreateTableAsync(TableName name, PasswordHash passwordHash, MaxPlayers maxPlayers, Stake stake, UserId createdBy)
    {
        var table = new Table(name, passwordHash, maxPlayers, stake, createdBy);
        await _tableRepository.AddAsync(table);
        return table;
    }

    public async Task<Table> JoinTableAsync(TableName tableName, PasswordHash tablePassword, UserId userId)
    {
        var table = await _tableRepository.GetByNameAsync(tableName) ?? throw new Exception("Table not found");
        table.AddMember(userId, false);
        await _tableRepository.UpdateAsync(table);
        return table;
    }

    public async Task LeaveTableAsync(TableId tableId, UserId userId)
    {
        var table = await _tableRepository.GetByIdAsync(tableId) ?? throw new Exception("Table not found");
        table.RemoveMember(userId);
        await _tableRepository.UpdateAsync(table);
    }

    public async Task GrantAdminRoleAsync(TableId tableId, UserId targetUserId, UserId adminUserId)
    {
        var table = await _tableRepository.GetByIdAsync(tableId) ?? throw new Exception("Table not found");
        table.GrantAdminRole(targetUserId);
        await _tableRepository.UpdateAsync(table);
    }

    public async Task RevokeAdminRoleAsync(TableId tableId, UserId targetUserId, UserId adminUserId)
    {
        var table = await _tableRepository.GetByIdAsync(tableId) ?? throw new Exception("Table not found");
        table.RevokeAdminRole(targetUserId);
        await _tableRepository.UpdateAsync(table);
    }

    public async Task UpdateTableSettingsAsync(TableId tableId, TableName name, MaxPlayers maxPlayers, Stake stake, bool isSecretMode, UserId adminUserId)
    {
        var table = await _tableRepository.GetByIdAsync(tableId) ?? throw new Exception("Table not found");
        table.UpdateSettings(name, maxPlayers, stake, isSecretMode);
        await _tableRepository.UpdateAsync(table);
    }

    public async Task<IEnumerable<Table>> GetUserTablesAsync(UserId userId)
    {
        return await _tableRepository.GetByUserIdAsync(userId);
    }

    public async Task<Table> GetTableDetailsAsync(TableId tableId, UserId userId)
    {
        return await _tableRepository.GetByIdAsync(tableId) ?? throw new Exception("Table not found");
    }

    public async Task<bool> IsUserMemberAsync(TableId tableId, UserId userId)
    {
        var table = await _tableRepository.GetByIdAsync(tableId);
        return table?.IsMember(userId) ?? false;
    }

    public async Task<bool> IsUserAdminAsync(TableId tableId, UserId userId)
    {
        var table = await _tableRepository.GetByIdAsync(tableId);
        return table?.IsAdmin(userId) ?? false;
    }

    public async Task<IEnumerable<TableMembership>> GetTableMembersAsync(TableId tableId, UserId userId)
    {
        var table = await _tableRepository.GetByIdAsync(tableId) ?? throw new Exception("Table not found");
        return table.Memberships;
    }
} 