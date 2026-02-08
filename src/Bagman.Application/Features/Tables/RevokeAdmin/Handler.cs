using Bagman.Application.Common;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.Tables.RevokeAdmin;

public record RevokeAdminCommand
{
    public required Guid TableId { get; init; }
    public required Guid RequestingUserId { get; init; }
    public required Guid TargetUserId { get; init; }
}

public class RevokeAdminHandler : IFeatureHandler<RevokeAdminCommand, Success>
{
    private readonly ITableRepository _tableRepository;

    public RevokeAdminHandler(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<ErrorOr<Success>> HandleAsync(
        RevokeAdminCommand request,
        CancellationToken cancellationToken = default)
    {
        // Get table aggregate
        var tableResult = await _tableRepository.GetByIdAsync(request.TableId);
        if (tableResult.IsError)
            return tableResult.Errors;

        if (tableResult.Value == null)
            return Error.NotFound("Table.NotFound", "Stół nie został znaleziony");

        var table = tableResult.Value;

        // Revoke admin through aggregate
        var revokeResult = table.RevokeAdmin(request.RequestingUserId, request.TargetUserId);
        if (revokeResult.IsError)
            return revokeResult.Errors;

        // Persist changes (EF change tracking automatically detects changes)
        var saveResult = await _tableRepository.SaveChangesAsync();

        if (saveResult.IsError)
            return saveResult.Errors;

        return Result.Success;
    }
}
