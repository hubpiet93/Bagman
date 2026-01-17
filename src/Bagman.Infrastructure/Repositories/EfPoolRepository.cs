using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using Bagman.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Infrastructure.Repositories;

public class EfPoolRepository : IPoolRepository
{
    private readonly ApplicationDbContext _db;

    public EfPoolRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<Pool?>> GetByIdAsync(Guid id)
    {
        try
        {
            var pool = await _db.Pools
                .Include(p => p.Winners)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            return pool;
        }
        catch (Exception ex)
        {
            return Error.Failure("Pool.GetByIdError", $"Błąd podczas pobierania puli: {ex.Message}");
        }
    }

    public async Task<ErrorOr<List<Pool>>> GetByMatchIdAsync(Guid matchId)
    {
        try
        {
            var pools = await _db.Pools
                .Where(p => p.MatchId == matchId)
                .Include(p => p.Winners)
                .AsNoTracking()
                .ToListAsync();

            return pools;
        }
        catch (Exception ex)
        {
            return Error.Failure("Pool.GetByMatchIdError", $"Błąd podczas pobierania puli meczu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Pool>> CreateAsync(Pool pool)
    {
        try
        {
            _db.Pools.Add(pool);
            await _db.SaveChangesAsync();
            return pool;
        }
        catch (Exception ex)
        {
            return Error.Failure("Pool.CreateError", $"Błąd podczas tworzenia puli: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Pool>> UpdateAsync(Pool pool)
    {
        try
        {
            _db.Pools.Update(pool);
            await _db.SaveChangesAsync();
            return pool;
        }
        catch (Exception ex)
        {
            return Error.Failure("Pool.UpdateError", $"Błąd podczas aktualizacji puli: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Success>> AddWinnerAsync(PoolWinner winner)
    {
        try
        {
            _db.PoolWinners.Add(winner);
            await _db.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("Pool.AddWinnerError", $"Błąd podczas dodawania zwycięzcy puli: {ex.Message}");
        }
    }

    public async Task<ErrorOr<List<PoolWinner>>> GetWinnersAsync(Guid poolId)
    {
        try
        {
            var winners = await _db.PoolWinners
                .Where(pw => pw.PoolId == poolId)
                .Include(pw => pw.User)
                .AsNoTracking()
                .ToListAsync();

            return winners;
        }
        catch (Exception ex)
        {
            return Error.Failure("Pool.GetWinnersError", $"Błąd podczas pobierania zwycięzców puli: {ex.Message}");
        }
    }
}
