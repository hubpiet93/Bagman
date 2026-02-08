using Bagman.Application.Common;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.EventTypes.GetActiveEventTypes;

public record EventTypeDto
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required DateTime StartDate { get; init; }
    public required bool IsActive { get; init; }
}

public record GetEventTypesResult
{
    public required IEnumerable<EventTypeDto> EventTypes { get; init; }
}

public record GetActiveEventTypesQuery;

public class GetActiveEventTypesHandler : IFeatureHandler<GetActiveEventTypesQuery, GetEventTypesResult>
{
    private readonly IEventTypeRepository _repo;

    public GetActiveEventTypesHandler(IEventTypeRepository repo)
    {
        _repo = repo;
    }

    public async Task<ErrorOr<GetEventTypesResult>> HandleAsync(
        GetActiveEventTypesQuery request,
        CancellationToken cancellationToken = default)
    {
        var eventTypes = await _repo.GetActiveAsync();

        var dtos = eventTypes.Select(et => new EventTypeDto
        {
            Id = et.Id,
            Code = et.Code,
            Name = et.Name,
            StartDate = et.StartDate,
            IsActive = et.IsActive
        });

        return new GetEventTypesResult
        {
            EventTypes = dtos
        };
    }
}
