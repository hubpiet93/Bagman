using Bagman.Application.Common;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.Tables.GrantAdmin;

public record GrantAdminCommand
{
    public required Guid TableId { get; init; }
    public required Guid RequestingUserId { get; init; }
    public required Guid TargetUserId { get; init; }
}

public class GrantAdminHandler : IFeatureHandler<GrantAdminCommand, Success>
{
    private readonly ITableRepository _tableRepository;

    public GrantAdminHandler(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<ErrorOr<Success>> HandleAsync(
        GrantAdminCommand request,
        CancellationToken cancellationToken = default)
    {
        // Get table aggregate
        var tableResult = await _tableRepository.GetByIdAsync(request.TableId);
        if (tableResult.IsError)
            return tableResult.Errors;

        if (tableResult.Value == null)
            return Error.NotFound("Table.NotFound", "Stół nie został znaleziony");

        var table = tableResult.Value;

        // Grant admin through aggregate
        var grantResult = table.GrantAdmin(request.RequestingUserId, request.TargetUserId);
        if (grantResult.IsError)
            return grantResult.Errors;

        // Persist changes (EF change tracking automatically detects changes)
        var saveResult = await _tableRepository.SaveChangesAsync();
        
        if (saveResult.IsError)
            return saveResult.Errors;

        return Result.Success;
    }
}
