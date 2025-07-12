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
        return await _tableRepository.IsUserMemberAsync(tableId, userId);
    }

    public async Task<bool> IsUserAdminAsync(TableId tableId, UserId userId)
    {
        return await _tableRepository.IsUserAdminAsync(tableId, userId);
    }

    public async Task<IEnumerable<TableMembership>> GetTableMembersAsync(TableId tableId, UserId userId)
    {
        return await _tableRepository.GetMembershipsAsync(tableId);
    }

    public async Task<bool> CanUserJoinTableAsync(TableId tableId, UserId userId)
    {
        var table = await _tableRepository.GetByIdAsync(tableId);
        if (table == null) return false;

        var isAlreadyMember = await _tableRepository.IsUserMemberAsync(tableId, userId);
        if (isAlreadyMember) return false;

        var memberCount = await _tableRepository.GetMemberCountAsync(tableId);
        return memberCount < table.MaxPlayers.Value;
    }

    public async Task<bool> CanUserLeaveTableAsync(TableId tableId, UserId userId)
    {
        var table = await _tableRepository.GetByIdAsync(tableId);
        if (table == null) return false;

        var isMember = await _tableRepository.IsUserMemberAsync(tableId, userId);
        if (!isMember) return false;

        var isAdmin = await _tableRepository.IsUserAdminAsync(tableId, userId);
        var adminCount = (await _tableRepository.GetMembershipsAsync(tableId)).Count(m => m.IsAdmin);

        // Prevent leaving if user is the only admin
        return !isAdmin || adminCount > 1;
    }

    public async Task<bool> CanUserDeleteTableAsync(TableId tableId, UserId userId)
    {
        var table = await _tableRepository.GetByIdAsync(tableId);
        if (table == null) return false;

        var isAdmin = await _tableRepository.IsUserAdminAsync(tableId, userId);
        var isCreator = table.CreatedBy == userId;

        return isAdmin && isCreator;
    }

    public async Task<bool> CanUserEditTableAsync(TableId tableId, UserId userId)
    {
        var table = await _tableRepository.GetByIdAsync(tableId);
        if (table == null) return false;

        var isAdmin = await _tableRepository.IsUserAdminAsync(tableId, userId);
        return isAdmin;
    }

    public async Task<bool> IsTableFullAsync(TableId tableId)
    {
        var table = await _tableRepository.GetByIdAsync(tableId);
        if (table == null) return true;

        var memberCount = await _tableRepository.GetMemberCountAsync(tableId);
        return memberCount >= table.MaxPlayers.Value;
    }

    public async Task<bool> IsTableSecretAsync(TableId tableId)
    {
        var table = await _tableRepository.GetByIdAsync(tableId);
        return table?.IsSecretMode ?? false;
    }

    public async Task<bool> ValidateTablePasswordAsync(TableId tableId, string password)
    {
        var table = await _tableRepository.GetByIdAsync(tableId);
        if (table == null) return false;

        // In a real implementation, you would hash the password and compare
        // For now, we'll assume the password is stored as a hash
        return table.PasswordHash.Value == password; // This should be proper password verification
    }
} 