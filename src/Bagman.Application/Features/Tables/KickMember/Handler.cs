using Bagman.Application.Common;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.Tables.KickMember;

public record KickMemberCommand
{
    public required Guid TableId { get; init; }
    public required Guid RequestingUserId { get; init; }
    public required Guid TargetUserId { get; init; }
}

public class KickMemberHandler : IFeatureHandler<KickMemberCommand, Success>
{
    private readonly ITableRepository _tableRepository;

    public KickMemberHandler(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<ErrorOr<Success>> HandleAsync(
        KickMemberCommand request,
        CancellationToken cancellationToken = default)
    {
        var tableResult = await _tableRepository.GetByIdAsync(request.TableId);
        if (tableResult.IsError)
            return tableResult.Errors;

        if (tableResult.Value == null)
            return Error.NotFound("Table.NotFound", "Stół nie został znaleziony");

        var table = tableResult.Value;

        if (!table.IsUserAdmin(request.RequestingUserId))
            return Error.Forbidden("Table.NotAdmin", "Nie masz uprawnień do zarządzania członkami stołu");

        if (request.TargetUserId == table.CreatedBy)
            return Error.Forbidden("Table.CannotKickCreator", "Nie można usunąć twórcy stołu");

        var removeMemberResult = table.RemoveMember(request.TargetUserId);
        if (removeMemberResult.IsError)
            return removeMemberResult.Errors;

        var saveResult = await _tableRepository.SaveChangesAsync();
        if (saveResult.IsError)
            return saveResult.Errors;

        return Result.Success;
    }
}
