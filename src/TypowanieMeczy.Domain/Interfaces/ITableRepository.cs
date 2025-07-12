using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Interfaces;

public interface ITableRepository
{
    Task<Table?> GetByIdAsync(TableId id);
    Task<Table?> GetByNameAsync(TableName name);
    Task<IEnumerable<Table>> GetByUserIdAsync(UserId userId);
    Task<IEnumerable<Table>> GetAllAsync();
    Task<bool> ExistsAsync(TableId id);
    Task<bool> ExistsByNameAsync(TableName name);
    Task AddAsync(Table table);
    Task UpdateAsync(Table table);
    Task DeleteAsync(TableId id);
    Task<bool> IsUserMemberAsync(TableId tableId, UserId userId);
    Task<bool> IsUserAdminAsync(TableId tableId, UserId userId);
    Task<int> GetMemberCountAsync(TableId tableId);
    Task<IEnumerable<TableMembership>> GetMembershipsAsync(TableId tableId);
} 