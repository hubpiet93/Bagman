using Bagman.Application.Common;
using Bagman.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Application.Features.Tables.GetTableByName;

public record GetTableByNameQuery
{
    public required string TableName { get; init; }
}

public record TableBasicResult
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int MaxPlayers { get; init; }
    public required decimal Stake { get; init; }
    public required Guid CreatedBy { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required bool IsSecretMode { get; init; }
}

public class GetTableByNameHandler : IFeatureHandler<GetTableByNameQuery, TableBasicResult?>
{
    private readonly ApplicationDbContext _dbContext;

    public GetTableByNameHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<TableBasicResult?>> HandleAsync(
        GetTableByNameQuery request,
        CancellationToken cancellationToken = default)
    {
        var table = await _dbContext.Tables
            .Where(t => t.Name.Value == request.TableName)
            .Select(t => new TableBasicResult
            {
                Id = t.Id,
                Name = t.Name.Value,
                MaxPlayers = t.MaxPlayers,
                Stake = t.Stake.Amount,
                CreatedBy = t.CreatedBy,
                CreatedAt = t.CreatedAt,
                IsSecretMode = t.IsSecretMode
            })
            .FirstOrDefaultAsync(cancellationToken);

        return table;
    }
}
