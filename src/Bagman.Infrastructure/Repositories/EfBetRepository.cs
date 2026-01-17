using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using Bagman.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Infrastructure.Repositories;

public class EfBetRepository : IBetRepository
{
    private readonly ApplicationDbContext _db;

    public EfBetRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<Bet?>> GetByIdAsync(Guid id)
    {
        try
        {
            var bet = await _db.Bets
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            return bet;
        }
        catch (Exception ex)
        {
            return Error.Failure("Bet.GetByIdError", $"Błąd podczas pobierania typu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Bet?>> GetByUserAndMatchAsync(Guid userId, Guid matchId)
    {
        try
        {
            var bet = await _db.Bets
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.UserId == userId && b.MatchId == matchId);

            return bet;
        }
        catch (Exception ex)
        {
            return Error.Failure("Bet.GetByUserAndMatchError", $"Błąd podczas pobierania typu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<List<Bet>>> GetByMatchIdAsync(Guid matchId)
    {
        try
        {
            var bets = await _db.Bets
                .Where(b => b.MatchId == matchId)
                .AsNoTracking()
                .ToListAsync();

            return bets;
        }
        catch (Exception ex)
        {
            return Error.Failure("Bet.GetByMatchIdError", $"Błąd podczas pobierania typów na mecz: {ex.Message}");
        }
    }

    public async Task<ErrorOr<List<Bet>>> GetByUserIdAsync(Guid userId)
    {
        try
        {
            var bets = await _db.Bets
                .Where(b => b.UserId == userId)
                .AsNoTracking()
                .ToListAsync();

            return bets;
        }
        catch (Exception ex)
        {
            return Error.Failure("Bet.GetByUserIdError", $"Błąd podczas pobierania typów użytkownika: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Bet>> CreateAsync(Bet bet)
    {
        try
        {
            _db.Bets.Add(bet);
            await _db.SaveChangesAsync();
            return bet;
        }
        catch (Exception ex)
        {
            return Error.Failure("Bet.CreateError", $"Błąd podczas tworzenia typu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Bet>> UpdateAsync(Bet bet)
    {
        try
        {
            _db.Bets.Update(bet);
            await _db.SaveChangesAsync();
            return bet;
        }
        catch (Exception ex)
        {
            return Error.Failure("Bet.UpdateError", $"Błąd podczas aktualizacji typu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Success>> DeleteAsync(Guid id)
    {
        try
        {
            var bet = await _db.Bets.FirstOrDefaultAsync(b => b.Id == id);
            if (bet == null)
                return Error.NotFound("Bet.NotFound", "Typ nie został znaleziony");

            _db.Bets.Remove(bet);
            await _db.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("Bet.DeleteError", $"Błąd podczas usuwania typu: {ex.Message}");
        }
    }
}
