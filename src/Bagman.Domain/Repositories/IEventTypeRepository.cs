using Bagman.Domain.Models;

namespace Bagman.Domain.Repositories;

public interface IEventTypeRepository : IRepository<EventType>
{
    Task<List<EventType>> GetActiveAsync();
    Task<List<EventType>> GetAllAsync();
    Task<EventType?> GetByCodeAsync(string code);
}
