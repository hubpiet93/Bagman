using Bagman.Application.Common;
using Bagman.Domain.Common.ValueObjects;
using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.Matches.CreateMatch;

public record CreateMatchCommand
{
    public required Guid EventTypeId { get; init; }
    public required string Country1 { get; init; }
    public required string Country2 { get; init; }
    public required DateTime MatchDateTime { get; init; }
    public required Guid UserId { get; init; }
}

public record CreateMatchResult
{
    public required Guid Id { get; init; }
    public required Guid EventTypeId { get; init; }
    public required string Country1 { get; init; }
    public required string Country2 { get; init; }
    public required DateTime MatchDateTime { get; init; }
    public required string Status { get; init; }
    public required DateTime CreatedAt { get; init; }
}

public class CreateMatchHandler : IFeatureHandler<CreateMatchCommand, CreateMatchResult>
{
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;

    public CreateMatchHandler(
        IMatchRepository matchRepository,
        IUserRepository userRepository,
        IEventTypeRepository eventTypeRepository)
    {
        _matchRepository = matchRepository;
        _userRepository = userRepository;
        _eventTypeRepository = eventTypeRepository;
    }

    public async Task<ErrorOr<CreateMatchResult>> HandleAsync(
        CreateMatchCommand request,
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

        // Create value objects
        var country1Result = Country.Create(request.Country1);
        if (country1Result.IsError)
            return country1Result.Errors;

        var country2Result = Country.Create(request.Country2);
        if (country2Result.IsError)
            return country2Result.Errors;

        // Create match aggregate
        var matchResult = Match.Create(
            request.EventTypeId,
            country1Result.Value,
            country2Result.Value,
            request.MatchDateTime);

        if (matchResult.IsError)
            return matchResult.Errors;

        // Persist
        _matchRepository.Add(matchResult.Value);
        var saveResult = await _matchRepository.SaveChangesAsync();

        if (saveResult.IsError)
            return saveResult.Errors;

        var match = matchResult.Value;
        return new CreateMatchResult
        {
            Id = match.Id,
            EventTypeId = match.EventTypeId,
            Country1 = match.Country1.Name,
            Country2 = match.Country2.Name,
            MatchDateTime = match.MatchDateTime,
            Status = match.Status,
            CreatedAt = match.CreatedAt
        };
    }
}
