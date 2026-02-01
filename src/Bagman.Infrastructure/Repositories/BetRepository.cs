using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using Bagman.Infrastructure.Data;

namespace Bagman.Infrastructure.Repositories;

public class BetRepository : Repository<Bet>, IBetRepository
{
    public BetRepository(ApplicationDbContext context) : base(context)
    {
    }
}
