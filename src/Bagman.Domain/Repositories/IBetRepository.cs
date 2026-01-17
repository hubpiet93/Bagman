using Bagman.Domain.Models;
using ErrorOr;

namespace Bagman.Domain.Repositories;

public interface IBetRepository
{
    Task<ErrorOr<Bet?>> GetByIdAsync(Guid id);
    Task<ErrorOr<Bet?>> GetByUserAndMatchAsync(Guid userId, Guid matchId);
    Task<ErrorOr<List<Bet>>> GetByMatchIdAsync(Guid matchId);
    Task<ErrorOr<List<Bet>>> GetByUserIdAsync(Guid userId);
    Task<ErrorOr<Bet>> CreateAsync(Bet bet);
    Task<ErrorOr<Bet>> UpdateAsync(Bet bet);
    Task<ErrorOr<Success>> DeleteAsync(Guid id);
}
