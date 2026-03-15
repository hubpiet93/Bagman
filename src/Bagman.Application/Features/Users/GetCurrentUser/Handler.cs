using Bagman.Application.Common;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.Users.GetCurrentUser;

public record GetCurrentUserQuery
{
    public required Guid UserId { get; init; }
}

public record CurrentUserResult
{
    public required Guid Id { get; init; }
    public required string Login { get; init; }
    public required string Email { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required bool IsActive { get; init; }
    public required bool IsSuperAdmin { get; init; }
}

public class GetCurrentUserHandler : IFeatureHandler<GetCurrentUserQuery, CurrentUserResult>
{
    private readonly IUserRepository _userRepository;

    public GetCurrentUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<CurrentUserResult>> HandleAsync(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken = default)
    {
        var userResult = await _userRepository.GetByIdAsync(request.UserId);
        if (userResult.IsError)
            return userResult.Errors;

        if (userResult.Value == null)
            return Error.NotFound("User.NotFound", "Użytkownik nie został znaleziony");

        var user = userResult.Value;

        return new CurrentUserResult
        {
            Id = user.Id,
            Login = user.Login,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive,
            IsSuperAdmin = user.IsSuperAdmin
        };
    }
}
