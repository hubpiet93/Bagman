using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using Bagman.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Infrastructure.Repositories;

public class MatchRepository : Repository<Match>, IMatchRepository
{
    public MatchRepository(ApplicationDbContext context) : base(context)
    {
    }

    // Override to load owned Bets collection
    public override async Task<ErrorOr<Match?>> GetByIdAsync(Guid id)
    {
        try
        {
            // EF Core REQUIRES Include() for OwnsMany collections!
            // Unlike OwnsOne, OwnsMany is NOT automatically loaded
            var match = await Context.Matches
                .Include(m => m.Bets)  // CRITICAL: Load owned Bets collection
                .AsTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            return match;
        }
        catch (Exception ex)
        {
            return Error.Failure("Database.Error", ex.Message);
        }
    }
}
