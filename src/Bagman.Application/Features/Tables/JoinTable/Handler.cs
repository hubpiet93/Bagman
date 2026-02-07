using Bagman.Application.Common;
using Bagman.Domain.Repositories;
using Bagman.Domain.Services;
using ErrorOr;

namespace Bagman.Application.Features.Tables.JoinTable;

public record JoinTableCommand
{
    public required Guid TableId { get; init; }
    public required Guid UserId { get; init; }
    public required string Password { get; init; }
}

public class JoinTableHandler : IFeatureHandler<JoinTableCommand, Success>
{
    private readonly ITableRepository _tableRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public JoinTableHandler(
        ITableRepository tableRepository,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _tableRepository = tableRepository;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ErrorOr<Success>> HandleAsync(
        JoinTableCommand request,
        CancellationToken cancellationToken = default)
    {
        // Verify user exists
        var userResult = await _userRepository.GetByIdAsync(request.UserId);
        if (userResult.IsError)
            return userResult.Errors;

        if (userResult.Value == null)
            return Error.NotFound("User.NotFound", "Użytkownik nie został znaleziony");

        // Get table aggregate
        var tableResult = await _tableRepository.GetByIdAsync(request.TableId);
        if (tableResult.IsError)
            return tableResult.Errors;

        if (tableResult.Value == null)
            return Error.NotFound("Table.NotFound", "Stół nie został znaleziony");

        var table = tableResult.Value;

        // Verify password
        if (!_passwordHasher.VerifyPassword(table.PasswordHash, request.Password))
        {
            return Error.Forbidden("Table.InvalidPassword", "Nieprawidłowe hasło do stołu");
        }

        // Add member through aggregate
        var addMemberResult = table.AddMember(request.UserId, request.Password);
        if (addMemberResult.IsError)
            return addMemberResult.Errors;

        // Persist changes (EF change tracking automatically detects changes)
        var saveResult = await _tableRepository.SaveChangesAsync();
        
        if (saveResult.IsError)
            return saveResult.Errors;

        return Result.Success;
    }
}
