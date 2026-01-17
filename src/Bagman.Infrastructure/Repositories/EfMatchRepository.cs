using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using Bagman.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Infrastructure.Repositories;

public class EfMatchRepository : IMatchRepository
{
    private readonly ApplicationDbContext _db;

    public EfMatchRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<Match?>> GetByIdAsync(Guid id)
    {
        try
        {
            var match = await _db.Matches
                .Include(m => m.Bets)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            return match;
        }
        catch (Exception ex)
        {
            return Error.Failure("Match.GetByIdError", $"Błąd podczas pobierania meczu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<List<Match>>> GetByTableIdAsync(Guid tableId)
    {
        try
        {
            var matches = await _db.Matches
                .Where(m => m.TableId == tableId)
                .OrderBy(m => m.MatchDateTime)
                .AsNoTracking()
                .ToListAsync();

            return matches;
        }
        catch (Exception ex)
        {
            return Error.Failure("Match.GetByTableIdError", $"Błąd podczas pobierania meczów: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Match>> CreateAsync(Match match)
    {
        try
        {
            _db.Matches.Add(match);
            await _db.SaveChangesAsync();
            return match;
        }
        catch (Exception ex)
        {
            return Error.Failure("Match.CreateError", $"Błąd podczas tworzenia meczu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Match>> UpdateAsync(Match match)
    {
        try
        {
            _db.Matches.Update(match);
            await _db.SaveChangesAsync();
            return match;
        }
        catch (Exception ex)
        {
            return Error.Failure("Match.UpdateError", $"Błąd podczas aktualizacji meczu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Success>> DeleteAsync(Guid id)
    {
        try
        {
            var match = await _db.Matches.FirstOrDefaultAsync(m => m.Id == id);
            if (match == null)
                return Error.NotFound("Match.NotFound", "Mecz nie został znaleziony");

            _db.Matches.Remove(match);
            await _db.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("Match.DeleteError", $"Błąd podczas usuwania meczu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<List<Match>>> GetUpcomingMatchesAsync(Guid tableId)
    {
        try
        {
            var now = DateTime.UtcNow;
            var matches = await _db.Matches
                .Where(m => m.TableId == tableId && m.MatchDateTime > now && m.Status == "scheduled")
                .OrderBy(m => m.MatchDateTime)
                .AsNoTracking()
                .ToListAsync();

            return matches;
        }
        catch (Exception ex)
        {
            return Error.Failure("Match.GetUpcomingError", $"Błąd podczas pobierania nadchodzących meczów: {ex.Message}");
        }
    }

    public async Task<ErrorOr<List<Match>>> GetFinishedMatchesAsync(Guid tableId)
    {
        try
        {
            var matches = await _db.Matches
                .Where(m => m.TableId == tableId && m.Status == "finished")
                .OrderByDescending(m => m.MatchDateTime)
                .AsNoTracking()
                .ToListAsync();

            return matches;
        }
        catch (Exception ex)
        {
            return Error.Failure("Match.GetFinishedError", $"Błąd podczas pobierania skończonych meczów: {ex.Message}");
        }
    }
}
