using Bagman.Application.Common;
using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.EventTypes.UpdateEventType;

public record UpdateEventTypeCommand
{
    public required Guid Id { get; init; }
    public string? Name { get; init; }
    public DateTime? StartDate { get; init; }
}

public record UpdateEventTypeResult
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required DateTime StartDate { get; init; }
    public required bool IsActive { get; init; }
}

public class UpdateEventTypeHandler : IFeatureHandler<UpdateEventTypeCommand, UpdateEventTypeResult>
{
    private readonly IEventTypeRepository _repo;

    public UpdateEventTypeHandler(IEventTypeRepository repo)
    {
        _repo = repo;
    }

    public async Task<ErrorOr<UpdateEventTypeResult>> HandleAsync(
        UpdateEventTypeCommand request,
        CancellationToken cancellationToken = default)
    {
        var eventTypeResult = await _repo.GetByIdAsync(request.Id);
        if (eventTypeResult.Value is not EventType eventType)
            return Error.NotFound("EventType.NotFound", "Nie znaleziono typu wydarzenia");

        if (!string.IsNullOrWhiteSpace(request.Name))
            eventType.Name = request.Name;
        if (request.StartDate.HasValue)
            eventType.StartDate = request.StartDate.Value;

        _repo.Update(eventType);
        await _repo.SaveChangesAsync();
        
        return new UpdateEventTypeResult
        {
            Id = eventType.Id,
            Code = eventType.Code,
            Name = eventType.Name,
            StartDate = eventType.StartDate,
            IsActive = eventType.IsActive
        };
    }
}
