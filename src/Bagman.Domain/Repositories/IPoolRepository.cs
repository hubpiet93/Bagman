using Bagman.Domain.Models;
using ErrorOr;

namespace Bagman.Domain.Repositories;

public interface IPoolRepository
{
    Task<ErrorOr<Pool?>> GetByIdAsync(Guid id);
    Task<ErrorOr<List<Pool>>> GetByMatchIdAsync(Guid matchId);
    Task<ErrorOr<Pool>> CreateAsync(Pool pool);
    Task<ErrorOr<Pool>> UpdateAsync(Pool pool);
    Task<ErrorOr<Success>> AddWinnerAsync(PoolWinner winner);
    Task<ErrorOr<List<PoolWinner>>> GetWinnersAsync(Guid poolId);
}
