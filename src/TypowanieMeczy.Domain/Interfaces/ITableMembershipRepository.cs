using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Interfaces;

public interface ITableMembershipRepository
{
    Task<TableMembership?> GetByUserAndTableAsync(UserId userId, TableId tableId);
    Task<IEnumerable<TableMembership>> GetByTableIdAsync(TableId tableId);
    Task<IEnumerable<TableMembership>> GetByUserIdAsync(UserId userId);
    Task<IEnumerable<TableMembership>> GetAdminsByTableIdAsync(TableId tableId);
    Task AddAsync(TableMembership membership);
    Task UpdateAsync(TableMembership membership);
    Task DeleteAsync(UserId userId, TableId tableId);
} 