using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Interfaces;

public interface IPoolRepository
{
    Task<Pool?> GetByIdAsync(PoolId id);
    Task<Pool?> GetByMatchIdAsync(MatchId matchId);
    Task<IEnumerable<Pool>> GetActivePoolsAsync();
    Task<IEnumerable<Pool>> GetExpiredPoolsAsync();
    Task AddAsync(Pool pool);
    Task UpdateAsync(Pool pool);
    Task DeleteAsync(PoolId id);
} 