using Bagman.Application.Common;
using Bagman.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Application.Features.Tables.GetTablePublicInfo;

public record GetTablePublicInfoQuery
{
    public required Guid TableId { get; init; }
}

public record TablePublicInfoResult
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int MembersCount { get; init; }
    public required int MaxPlayers { get; init; }
}

public class GetTablePublicInfoHandler : IFeatureHandler<GetTablePublicInfoQuery, TablePublicInfoResult>
{
    private readonly ApplicationDbContext _dbContext;

    public GetTablePublicInfoHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<TablePublicInfoResult>> HandleAsync(
        GetTablePublicInfoQuery request,
        CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.Tables
            .Where(t => t.Id == request.TableId)
            .Select(t => new TablePublicInfoResult
            {
                Id = t.Id,
                Name = t.Name.Value,
                MembersCount = t.Members.Count,
                MaxPlayers = t.MaxPlayers
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (result == null)
            return Error.NotFound("Table.NotFound", "Stół nie został znaleziony");

        return result;
    }
}
