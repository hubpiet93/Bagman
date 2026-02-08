using Bagman.Application.Common;
using Bagman.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Application.Features.Tables.GetUserTables;

public record GetUserTablesQuery
{
    public required Guid UserId { get; init; }
}

public record UserTableResult
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int MaxPlayers { get; init; }
    public required decimal Stake { get; init; }
    public required Guid CreatedBy { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required bool IsSecretMode { get; init; }
}

public class GetUserTablesHandler : IFeatureHandler<GetUserTablesQuery, List<UserTableResult>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetUserTablesHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<List<UserTableResult>>> HandleAsync(
        GetUserTablesQuery request,
        CancellationToken cancellationToken = default)
    {
        var tables = await _dbContext.Tables
            .Where(t => t.Members.Any(m => m.UserId == request.UserId))
            .OrderBy(t => t.CreatedAt)
            .Select(t => new UserTableResult
            {
                Id = t.Id,
                Name = t.Name.Value,
                MaxPlayers = t.MaxPlayers,
                Stake = t.Stake.Amount,
                CreatedBy = t.CreatedBy,
                CreatedAt = t.CreatedAt,
                IsSecretMode = t.IsSecretMode
            })
            .ToListAsync(cancellationToken);

        return tables;
    }
}
