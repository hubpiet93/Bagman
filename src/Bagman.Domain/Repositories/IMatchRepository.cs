using Bagman.Domain.Models;
using ErrorOr;

namespace Bagman.Domain.Repositories;

public interface IMatchRepository
{
    Task<ErrorOr<Match?>> GetByIdAsync(Guid id);
    Task<ErrorOr<List<Match>>> GetByTableIdAsync(Guid tableId);
    Task<ErrorOr<Match>> CreateAsync(Match match);
    Task<ErrorOr<Match>> UpdateAsync(Match match);
    Task<ErrorOr<Success>> DeleteAsync(Guid id);
    Task<ErrorOr<List<Match>>> GetUpcomingMatchesAsync(Guid tableId);
    Task<ErrorOr<List<Match>>> GetFinishedMatchesAsync(Guid tableId);
}
