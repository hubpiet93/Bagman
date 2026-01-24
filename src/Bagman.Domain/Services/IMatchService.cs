using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Domain.Services;

public interface IMatchService
{
    Task<ErrorOr<Match>> CreateMatchAsync(Guid tableId, string country1, string country2, DateTime matchDateTime, Guid userId);
    Task<ErrorOr<Match?>> GetMatchByIdAsync(Guid id);
    Task<ErrorOr<List<Match>>> GetTableMatchesAsync(Guid tableId);
    Task<ErrorOr<Success>> UpdateMatchAsync(Guid id, string country1, string country2, DateTime matchDateTime, Guid userId);
    Task<ErrorOr<Success>> DeleteMatchAsync(Guid id, Guid userId);
    Task<ErrorOr<Success>> SetMatchResultAsync(Guid id, string result, Guid userId);
}

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly ITableRepository _tableRepository;

    public MatchService(IMatchRepository matchRepository, ITableRepository tableRepository)
    {
        _matchRepository = matchRepository;
        _tableRepository = tableRepository;
    }

    public async Task<ErrorOr<Match>> CreateMatchAsync(Guid tableId, string country1, string country2, DateTime matchDateTime, Guid userId)
    {
        // Verify user is admin of the table
        var adminCheckResult = await VerifyUserIsTableAdminAsync(tableId, userId);
        if (adminCheckResult.IsError)
            return adminCheckResult.Errors;

        var match = new Match
        {
            Id = Guid.NewGuid(),
            TableId = tableId,
            Country1 = country1,
            Country2 = country2,
            MatchDateTime = matchDateTime,
            Status = "scheduled",
            CreatedAt = DateTime.UtcNow
        };

        return await _matchRepository.CreateAsync(match);
    }

    public async Task<ErrorOr<Match?>> GetMatchByIdAsync(Guid id)
    {
        return await _matchRepository.GetByIdAsync(id);
    }

    public async Task<ErrorOr<List<Match>>> GetTableMatchesAsync(Guid tableId)
    {
        return await _matchRepository.GetByTableIdAsync(tableId);
    }

    public async Task<ErrorOr<Success>> UpdateMatchAsync(Guid id, string country1, string country2, DateTime matchDateTime, Guid userId)
    {
        // Get match
        var matchResult = await _matchRepository.GetByIdAsync(id);
        if (matchResult.IsError)
            return matchResult.Errors;

        if (matchResult.Value == null)
            return Error.NotFound("Match.NotFound", "Mecz nie został znaleziony");

        var match = matchResult.Value;

        // Verify user is admin and match is not started
        var adminCheckResult = await VerifyUserIsTableAdminAsync(match.TableId, userId);
        if (adminCheckResult.IsError)
            return adminCheckResult.Errors;

        if (match.Started)
            return Error.Failure("Match.AlreadyStarted", "Nie można edytować meczu, który już się rozpoczął");

        match.Country1 = country1;
        match.Country2 = country2;
        match.MatchDateTime = matchDateTime;

        var updateResult = await _matchRepository.UpdateAsync(match);
        return updateResult.IsError ? updateResult.Errors : Result.Success;
    }

    public async Task<ErrorOr<Success>> DeleteMatchAsync(Guid id, Guid userId)
    {
        // Get match
        var matchResult = await _matchRepository.GetByIdAsync(id);
        if (matchResult.IsError)
            return matchResult.Errors;

        if (matchResult.Value == null)
            return Error.NotFound("Match.NotFound", "Mecz nie został znaleziony");

        var match = matchResult.Value;

        // Verify user is admin and match is not started
        var adminCheckResult = await VerifyUserIsTableAdminAsync(match.TableId, userId);
        if (adminCheckResult.IsError)
            return adminCheckResult.Errors;

        if (match.Started)
            return Error.Failure("Match.AlreadyStarted", "Nie można usunąć meczu, który już się rozpoczął");

        return await _matchRepository.DeleteAsync(id);
    }

    public async Task<ErrorOr<Success>> SetMatchResultAsync(Guid id, string result, Guid userId)
    {
        // Get match
        var matchResult = await _matchRepository.GetByIdAsync(id);
        if (matchResult.IsError)
            return matchResult.Errors;

        if (matchResult.Value == null)
            return Error.NotFound("Match.NotFound", "Mecz nie został znaleziony");

        var match = matchResult.Value;

        // Verify user is admin
        var adminCheckResult = await VerifyUserIsTableAdminAsync(match.TableId, userId);
        if (adminCheckResult.IsError)
            return adminCheckResult.Errors;

        match.Result = result;
        match.Status = "finished";

        var updateResult = await _matchRepository.UpdateAsync(match);
        return updateResult.IsError ? updateResult.Errors : Result.Success;
    }

    private async Task<ErrorOr<Success>> VerifyUserIsTableAdminAsync(Guid tableId, Guid userId)
    {
        var memberResult = await _tableRepository.GetMemberAsync(tableId, userId);
        if (memberResult.IsError)
            return memberResult.Errors;

        if (memberResult.Value == null || !memberResult.Value.IsAdmin)
            return Error.Forbidden("Table.NotAdmin", "Nie masz uprawnień do wykonania tej czynności");

        return Result.Success;
    }
}
