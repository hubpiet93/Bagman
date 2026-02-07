using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using Bagman.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Infrastructure.Repositories;

public class EfEventTypeRepository : IEventTypeRepository
{
    private readonly ApplicationDbContext _db;

    public EfEventTypeRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<EventType?>> GetByIdAsync(Guid id)
    {
        return await _db.EventTypes
            .Include(e => e.Tables)
            .Include(e => e.Matches)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public void Add(EventType entity)
    {
        _db.EventTypes.Add(entity);
    }

    public void Update(EventType entity)
    {
        _db.EventTypes.Update(entity);
    }

    public void Delete(EventType entity)
    {
        _db.EventTypes.Remove(entity);
    }

    public async Task<ErrorOr<Success>> SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
        return Result.Success;
    }

    public async Task<List<EventType>> GetActiveAsync()
    {
        return await _db.EventTypes
            .Where(e => e.IsActive)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<List<EventType>> GetAllAsync()
    {
        return await _db.EventTypes
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<EventType?> GetByCodeAsync(string code)
    {
        return await _db.EventTypes
            .FirstOrDefaultAsync(e => e.Code == code);
    }
}
