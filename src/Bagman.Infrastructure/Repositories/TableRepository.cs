using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using Bagman.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Infrastructure.Repositories;

public class TableRepository : Repository<Table>, ITableRepository
{
    public TableRepository(ApplicationDbContext context) : base(context)
    {
    }

    // Override to load owned Members collection
    public override async Task<ErrorOr<Table?>> GetByIdAsync(Guid id)
    {
        try
        {
            // EF Core REQUIRES Include() for OwnsMany collections!
            // Unlike OwnsOne, OwnsMany is NOT automatically loaded
            var table = await Context.Tables
                .Include(t => t.Members)  // CRITICAL: Load owned Members collection
                .AsTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
            return table;
        }
        catch (Exception ex)
        {
            return Error.Failure("Database.Error", ex.Message);
        }
    }
}
