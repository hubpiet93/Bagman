using Bagman.Application.Common;
using Bagman.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Application.Features.Tables.GetTableDetails;

public record GetTableDetailsQuery
{
    public required Guid TableId { get; init; }
}

public record TableDetailResult
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int MaxPlayers { get; init; }
    public required decimal Stake { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required List<TableMemberResult> Members { get; init; }
}

public record TableMemberResult
{
    public required Guid UserId { get; init; }
    public required string Login { get; init; }
    public required bool IsAdmin { get; init; }
    public required DateTime JoinedAt { get; init; }
}

public class GetTableDetailsHandler : IFeatureHandler<GetTableDetailsQuery, TableDetailResult>
{
    private readonly ApplicationDbContext _dbContext;

    public GetTableDetailsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<TableDetailResult>> HandleAsync(
        GetTableDetailsQuery request,
        CancellationToken cancellationToken = default)
    {
        var table = await _dbContext.Tables
            .Where(t => t.Id == request.TableId)
            .Select(t => new TableDetailResult
            {
                Id = t.Id,
                Name = t.Name.Value,
                MaxPlayers = t.MaxPlayers,
                Stake = t.Stake.Amount,
                CreatedAt = t.CreatedAt,
                Members = t.Members
                    .OrderBy(m => m.JoinedAt)
                    .Select(m => new TableMemberResult
                    {
                        UserId = m.UserId,
                        Login = m.User!.Login,
                        IsAdmin = m.IsAdmin,
                        JoinedAt = m.JoinedAt
                    }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (table == null)
            return Error.NotFound("Table.NotFound", "Stół nie został znaleziony");

        return table;
    }
}
