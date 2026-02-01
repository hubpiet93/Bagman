using Bagman.Application.Common;
using Bagman.Domain.Common.ValueObjects;
using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.Matches.CreateMatch;

public record CreateMatchCommand
{
    public required Guid TableId { get; init; }
    public required string Country1 { get; init; }
    public required string Country2 { get; init; }
    public required DateTime MatchDateTime { get; init; }
    public required Guid UserId { get; init; }
}

public record CreateMatchResult
{
    public required Guid Id { get; init; }
    public required Guid TableId { get; init; }
    public required string Country1 { get; init; }
    public required string Country2 { get; init; }
    public required DateTime MatchDateTime { get; init; }
    public required string Status { get; init; }
    public required DateTime CreatedAt { get; init; }
}

public class CreateMatchHandler : IFeatureHandler<CreateMatchCommand, CreateMatchResult>
{
    private readonly IMatchRepository _matchRepository;
    private readonly ITableRepository _tableRepository;

    public CreateMatchHandler(IMatchRepository matchRepository, ITableRepository tableRepository)
    {
        _matchRepository = matchRepository;
        _tableRepository = tableRepository;
    }

    public async Task<ErrorOr<CreateMatchResult>> HandleAsync(
        CreateMatchCommand request,
        CancellationToken cancellationToken = default)
    {
        // Verify user is admin of the table
        var tableResult = await _tableRepository.GetByIdAsync(request.TableId);
        if (tableResult.IsError)
            return tableResult.Errors;

        if (tableResult.Value == null)
            return Error.NotFound("Table.NotFound", "Stół nie został znaleziony");

        if (!tableResult.Value.IsUserAdmin(request.UserId))
            return Error.Forbidden("Table.NotAdmin", "Nie masz uprawnień do wykonania tej czynności");

        // Create value objects
        var country1Result = Country.Create(request.Country1);
        if (country1Result.IsError)
            return country1Result.Errors;

        var country2Result = Country.Create(request.Country2);
        if (country2Result.IsError)
            return country2Result.Errors;

        // Create match aggregate
        var matchResult = Match.Create(
            request.TableId,
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
            TableId = match.TableId,
            Country1 = match.Country1.Name,
            Country2 = match.Country2.Name,
            MatchDateTime = match.MatchDateTime,
            Status = match.Status,
            CreatedAt = match.CreatedAt
        };
    }
}
