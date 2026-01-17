using Bagman.Domain.Models;
using ErrorOr;

namespace Bagman.Domain.Repositories;

public interface IUserStatsRepository
{
    Task<ErrorOr<UserStats?>> GetAsync(Guid userId, Guid tableId);
    Task<ErrorOr<List<UserStats>>> GetByUserIdAsync(Guid userId);
    Task<ErrorOr<List<UserStats>>> GetByTableIdAsync(Guid tableId);
    Task<ErrorOr<UserStats>> CreateAsync(UserStats stats);
    Task<ErrorOr<UserStats>> UpdateAsync(UserStats stats);
}
