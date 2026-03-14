using Bagman.Domain.Models;
using ErrorOr;

namespace Bagman.Domain.Repositories;

public interface IMatchRepository : IRepository<Match>
{
    Task<ErrorOr<List<Match>>> GetByEventTypeIdAsync(Guid eventTypeId);
}
