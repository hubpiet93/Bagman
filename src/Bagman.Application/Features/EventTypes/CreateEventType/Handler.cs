using Bagman.Application.Common;
using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.EventTypes.CreateEventType;

public record CreateEventTypeCommand
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required DateTime StartDate { get; init; }
}

public record CreateEventTypeResult
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required DateTime StartDate { get; init; }
    public required bool IsActive { get; init; }
    public required DateTime CreatedAt { get; init; }
}

public class CreateEventTypeHandler : IFeatureHandler<CreateEventTypeCommand, CreateEventTypeResult>
{
    private readonly IEventTypeRepository _repo;

    public CreateEventTypeHandler(IEventTypeRepository repo)
    {
        _repo = repo;
    }

    public async Task<ErrorOr<CreateEventTypeResult>> HandleAsync(
        CreateEventTypeCommand request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.Name))
            return Error.Validation("EventType.InvalidData", "Kod i nazwa są wymagane");

        var exists = await _repo.GetByCodeAsync(request.Code);
        if (exists != null)
            return Error.Conflict("EventType.CodeExists", "Kod wydarzenia już istnieje");

        var eventType = new EventType
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Name = request.Name,
            StartDate = request.StartDate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _repo.Add(eventType);
        await _repo.SaveChangesAsync();
        
        return new CreateEventTypeResult
        {
            Id = eventType.Id,
            Code = eventType.Code,
            Name = eventType.Name,
            StartDate = eventType.StartDate,
            IsActive = eventType.IsActive,
            CreatedAt = eventType.CreatedAt
        };
    }
}
