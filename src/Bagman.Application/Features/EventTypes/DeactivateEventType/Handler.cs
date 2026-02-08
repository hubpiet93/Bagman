using Bagman.Application.Common;
using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.EventTypes.DeactivateEventType;

public record DeactivateEventTypeCommand
{
    public required Guid Id { get; init; }
}

public class DeactivateEventTypeHandler : IFeatureHandler<DeactivateEventTypeCommand, Success>
{
    private readonly IEventTypeRepository _repo;

    public DeactivateEventTypeHandler(IEventTypeRepository repo)
    {
        _repo = repo;
    }

    public async Task<ErrorOr<Success>> HandleAsync(
        DeactivateEventTypeCommand request,
        CancellationToken cancellationToken = default)
    {
        var eventTypeResult = await _repo.GetByIdAsync(request.Id);
        if (eventTypeResult.Value is not EventType eventType)
            return Error.NotFound("EventType.NotFound", "Nie znaleziono typu wydarzenia");

        var result = eventType.Deactivate();
        if (result.IsError)
            return result.Errors;

        _repo.Update(eventType);
        await _repo.SaveChangesAsync();

        return Result.Success;
    }
}
