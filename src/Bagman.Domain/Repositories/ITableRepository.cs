using Bagman.Domain.Models;
using ErrorOr;

namespace Bagman.Domain.Repositories;

public interface ITableRepository
{
    Task<ErrorOr<Table?>> GetByIdAsync(Guid id);
    Task<ErrorOr<Table?>> GetByNameAsync(string name);
    Task<ErrorOr<List<Table>>> GetByUserIdAsync(Guid userId);
    Task<ErrorOr<Table>> CreateAsync(Table table);
    Task<ErrorOr<Table>> UpdateAsync(Table table);
    Task<ErrorOr<Success>> DeleteAsync(Guid id);
    Task<ErrorOr<List<TableMember>>> GetMembersAsync(Guid tableId);
    Task<ErrorOr<TableMember?>> GetMemberAsync(Guid tableId, Guid userId);
    Task<ErrorOr<Success>> AddMemberAsync(TableMember member);
    Task<ErrorOr<Success>> RemoveMemberAsync(Guid tableId, Guid userId);
    Task<ErrorOr<Success>> UpdateMemberAdminAsync(Guid tableId, Guid userId, bool isAdmin);
}
