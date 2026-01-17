using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using Bagman.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Infrastructure.Repositories;

public class EfUserStatsRepository : IUserStatsRepository
{
    private readonly ApplicationDbContext _db;

    public EfUserStatsRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<UserStats?>> GetAsync(Guid userId, Guid tableId)
    {
        try
        {
            var stats = await _db.UserStats
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId && s.TableId == tableId);

            return stats;
        }
        catch (Exception ex)
        {
            return Error.Failure("UserStats.GetError", $"Błąd podczas pobierania statystyk: {ex.Message}");
        }
    }

    public async Task<ErrorOr<List<UserStats>>> GetByUserIdAsync(Guid userId)
    {
        try
        {
            var stats = await _db.UserStats
                .Where(s => s.UserId == userId)
                .AsNoTracking()
                .ToListAsync();

            return stats;
        }
        catch (Exception ex)
        {
            return Error.Failure("UserStats.GetByUserIdError", $"Błąd podczas pobierania statystyk użytkownika: {ex.Message}");
        }
    }

    public async Task<ErrorOr<List<UserStats>>> GetByTableIdAsync(Guid tableId)
    {
        try
        {
            var stats = await _db.UserStats
                .Where(s => s.TableId == tableId)
                .Include(s => s.User)
                .AsNoTracking()
                .ToListAsync();

            return stats;
        }
        catch (Exception ex)
        {
            return Error.Failure("UserStats.GetByTableIdError", $"Błąd podczas pobierania statystyk stołu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<UserStats>> CreateAsync(UserStats stats)
    {
        try
        {
            _db.UserStats.Add(stats);
            await _db.SaveChangesAsync();
            return stats;
        }
        catch (Exception ex)
        {
            return Error.Failure("UserStats.CreateError", $"Błąd podczas tworzenia statystyk: {ex.Message}");
        }
    }

    public async Task<ErrorOr<UserStats>> UpdateAsync(UserStats stats)
    {
        try
        {
            _db.UserStats.Update(stats);
            await _db.SaveChangesAsync();
            return stats;
        }
        catch (Exception ex)
        {
            return Error.Failure("UserStats.UpdateError", $"Błąd podczas aktualizacji statystyk: {ex.Message}");
        }
    }
}
