using Bagman.Application.Common;
using Bagman.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Application.Features.Matches.GetMatchDetails;

public record GetMatchDetailsQuery
{
    public required Guid MatchId { get; init; }
}

public record MatchDetailsResult
{
    public required Guid Id { get; init; }
    public required Guid EventTypeId { get; init; }
    public required string Country1 { get; init; }
    public required string Country2 { get; init; }
    public required DateTime MatchDateTime { get; init; }
    public required string? Result { get; init; }
    public required string Status { get; init; }
    public required DateTime CreatedAt { get; init; }
}

public class GetMatchDetailsHandler : IFeatureHandler<GetMatchDetailsQuery, MatchDetailsResult>
{
    private readonly ApplicationDbContext _dbContext;

    public GetMatchDetailsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<MatchDetailsResult>> HandleAsync(
        GetMatchDetailsQuery request,
        CancellationToken cancellationToken = default)
    {
        var match = await _dbContext.Matches
            .Where(m => m.Id == request.MatchId)
            .Select(m => new MatchDetailsResult
            {
                Id = m.Id,
                EventTypeId = m.EventTypeId,
                Country1 = m.Country1.Name,
                Country2 = m.Country2.Name,
                MatchDateTime = m.MatchDateTime,
                Result = m.Result != null ? m.Result.Value : null,
                Status = m.Status,
                CreatedAt = m.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (match == null)
            return Error.NotFound("Match.NotFound", "Mecz nie został znaleziony");

        return match;
    }
}
