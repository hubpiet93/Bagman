using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Domain.Services;

public interface ITableService
{
    Task<ErrorOr<Table>> CreateTableAsync(string name, string passwordHash, int maxPlayers, decimal stake, Guid createdBy);
    Task<ErrorOr<Table?>> GetTableByIdAsync(Guid id);
    Task<ErrorOr<Table?>> GetTableByNameAsync(string name);
    Task<ErrorOr<List<Table>>> GetUserTablesAsync(Guid userId);
    Task<ErrorOr<Success>> JoinTableAsync(Guid tableId, Guid userId, string password);
    Task<ErrorOr<Success>> LeaveTableAsync(Guid tableId, Guid userId);
    Task<ErrorOr<Success>> GrantAdminAsync(Guid tableId, Guid userId, Guid grantedToUserId);
    Task<ErrorOr<Success>> RevokeAdminAsync(Guid tableId, Guid userId, Guid revokedFromUserId);
    Task<ErrorOr<List<TableMember>>> GetTableMembersAsync(Guid tableId);
}

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;
    private readonly IUserRepository _userRepository;

    public TableService(ITableRepository tableRepository, IUserRepository userRepository)
    {
        _tableRepository = tableRepository;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Table>> CreateTableAsync(string name, string passwordHash, int maxPlayers, decimal stake, Guid createdBy)
    {
        // Check if user exists
        var userResult = await _userRepository.GetByIdAsync(createdBy);
        if (userResult.IsError)
            return userResult.Errors;

        if (userResult.Value == null)
            return Error.NotFound("User.NotFound", "Użytkownik nie został znaleziony");

        var table = new Table
        {
            Id = Guid.NewGuid(),
            Name = name,
            PasswordHash = passwordHash,
            MaxPlayers = maxPlayers,
            Stake = stake,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            IsSecretMode = false
        };

        var createResult = await _tableRepository.CreateAsync(table);
        if (createResult.IsError)
            return createResult.Errors;

        // Add creator as table member with admin role
        var member = new TableMember
        {
            UserId = createdBy,
            TableId = createResult.Value.Id,
            IsAdmin = true,
            JoinedAt = DateTime.UtcNow
        };

        var addMemberResult = await _tableRepository.AddMemberAsync(member);
        if (addMemberResult.IsError)
            return addMemberResult.Errors;

        return createResult.Value;
    }

    public async Task<ErrorOr<Table?>> GetTableByIdAsync(Guid id)
    {
        return await _tableRepository.GetByIdAsync(id);
    }

    public async Task<ErrorOr<Table?>> GetTableByNameAsync(string name)
    {
        return await _tableRepository.GetByNameAsync(name);
    }

    public async Task<ErrorOr<List<Table>>> GetUserTablesAsync(Guid userId)
    {
        return await _tableRepository.GetByUserIdAsync(userId);
    }

    public async Task<ErrorOr<Success>> JoinTableAsync(Guid tableId, Guid userId, string password)
    {
        // Verify user exists
        var userResult = await _userRepository.GetByIdAsync(userId);
        if (userResult.IsError)
            return userResult.Errors;

        if (userResult.Value == null)
            return Error.NotFound("User.NotFound", "Użytkownik nie został znaleziony");

        // Get table
        var tableResult = await _tableRepository.GetByIdAsync(tableId);
        if (tableResult.IsError)
            return tableResult.Errors;

        if (tableResult.Value == null)
            return Error.NotFound("Table.NotFound", "Stół nie został znaleziony");

        var table = tableResult.Value;

        // Check if user is already a member
        var memberResult = await _tableRepository.GetMemberAsync(tableId, userId);
        if (memberResult.IsError)
            return memberResult.Errors;

        if (memberResult.Value != null)
            return Error.Conflict("Table.AlreadyMember", "Użytkownik jest już członkiem tego stołu");

        // Check member count
        var membersResult = await _tableRepository.GetMembersAsync(tableId);
        if (membersResult.IsError)
            return membersResult.Errors;

        if (membersResult.Value.Count >= table.MaxPlayers)
            return Error.Failure("Table.Full", "Stół jest pełny");

        // Verify password (in real implementation, compare with bcrypt hash)
        // For now, we assume password verification is done before calling this method

        var member = new TableMember
        {
            UserId = userId,
            TableId = tableId,
            IsAdmin = false,
            JoinedAt = DateTime.UtcNow
        };

        return await _tableRepository.AddMemberAsync(member);
    }

    public async Task<ErrorOr<Success>> LeaveTableAsync(Guid tableId, Guid userId)
    {
        return await _tableRepository.RemoveMemberAsync(tableId, userId);
    }

    public async Task<ErrorOr<Success>> GrantAdminAsync(Guid tableId, Guid userId, Guid grantedToUserId)
    {
        // Verify current user is admin
        var currentMemberResult = await _tableRepository.GetMemberAsync(tableId, userId);
        if (currentMemberResult.IsError)
            return currentMemberResult.Errors;

        if (currentMemberResult.Value == null || !currentMemberResult.Value.IsAdmin)
            return Error.Forbidden("Table.NotAdmin", "Nie masz uprawnień do zarządzania administratorami");

        // Verify target user is a member
        var targetMemberResult = await _tableRepository.GetMemberAsync(tableId, grantedToUserId);
        if (targetMemberResult.IsError)
            return targetMemberResult.Errors;

        if (targetMemberResult.Value == null)
            return Error.NotFound("Table.MemberNotFound", "Członek nie został znaleziony");

        return await _tableRepository.UpdateMemberAdminAsync(tableId, grantedToUserId, true);
    }

    public async Task<ErrorOr<Success>> RevokeAdminAsync(Guid tableId, Guid userId, Guid revokedFromUserId)
    {
        // Verify current user is admin
        var currentMemberResult = await _tableRepository.GetMemberAsync(tableId, userId);
        if (currentMemberResult.IsError)
            return currentMemberResult.Errors;

        if (currentMemberResult.Value == null || !currentMemberResult.Value.IsAdmin)
            return Error.Forbidden("Table.NotAdmin", "Nie masz uprawnień do zarządzania administratorami");

        // Verify target user is a member
        var targetMemberResult = await _tableRepository.GetMemberAsync(tableId, revokedFromUserId);
        if (targetMemberResult.IsError)
            return targetMemberResult.Errors;

        if (targetMemberResult.Value == null)
            return Error.NotFound("Table.MemberNotFound", "Członek nie został znaleziony");

        return await _tableRepository.UpdateMemberAdminAsync(tableId, revokedFromUserId, false);
    }

    public async Task<ErrorOr<List<TableMember>>> GetTableMembersAsync(Guid tableId)
    {
        return await _tableRepository.GetMembersAsync(tableId);
    }
}
