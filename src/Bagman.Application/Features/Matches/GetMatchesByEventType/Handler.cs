using Bagman.Application.Common;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.Matches.GetMatchesByEventType;

public record GetMatchesByEventTypeQuery
{
    public required Guid EventTypeId { get; init; }
    public required Guid UserId { get; init; }
}

public record MatchListItemResult
{
    public required Guid Id { get; init; }
    public required Guid EventTypeId { get; init; }
    public required string Country1 { get; init; }
    public required string Country2 { get; init; }
    public required DateTime MatchDateTime { get; init; }
    public required string Status { get; init; }
    public required DateTime CreatedAt { get; init; }
}

public class GetMatchesByEventTypeHandler
    : IFeatureHandler<GetMatchesByEventTypeQuery, List<MatchListItemResult>>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IUserRepository _userRepository;

    public GetMatchesByEventTypeHandler(
        IMatchRepository matchRepository,
        IEventTypeRepository eventTypeRepository,
        IUserRepository userRepository)
    {
        _matchRepository = matchRepository;
        _eventTypeRepository = eventTypeRepository;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<List<MatchListItemResult>>> HandleAsync(
        GetMatchesByEventTypeQuery request,
        CancellationToken cancellationToken = default)
    {
        // Verify user is SuperAdmin
        var userResult = await _userRepository.GetByIdAsync(request.UserId);
        if (userResult.IsError)
            return userResult.Errors;

        if (userResult.Value == null)
            return Error.NotFound("User.NotFound", "Użytkownik nie został znaleziony");

        if (!userResult.Value.IsSuperAdmin)
            return Error.Forbidden("User.NotSuperAdmin", "Nie masz uprawnień do zarządzania meczami");

        // Verify EventType exists
        var eventTypeResult = await _eventTypeRepository.GetByIdAsync(request.EventTypeId);
        if (eventTypeResult.IsError)
            return eventTypeResult.Errors;

        if (eventTypeResult.Value == null)
            return Error.NotFound("EventType.NotFound", "Typ wydarzenia nie został znaleziony");

        // Get matches
        var matchesResult = await _matchRepository.GetByEventTypeIdAsync(request.EventTypeId);
        if (matchesResult.IsError)
            return matchesResult.Errors;

        return matchesResult.Value.Select(m => new MatchListItemResult
        {
            Id = m.Id,
            EventTypeId = m.EventTypeId,
            Country1 = m.Country1.Name,
            Country2 = m.Country2.Name,
            MatchDateTime = m.MatchDateTime,
            Status = m.Status,
            CreatedAt = m.CreatedAt
        }).ToList();
    }
}
