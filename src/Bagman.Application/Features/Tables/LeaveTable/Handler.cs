using Bagman.Application.Common;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.Tables.LeaveTable;

public record LeaveTableCommand
{
    public required Guid TableId { get; init; }
    public required Guid UserId { get; init; }
}

public class LeaveTableHandler : IFeatureHandler<LeaveTableCommand, Success>
{
    private readonly ITableRepository _tableRepository;

    public LeaveTableHandler(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<ErrorOr<Success>> HandleAsync(
        LeaveTableCommand request,
        CancellationToken cancellationToken = default)
    {
        // Get table aggregate
        var tableResult = await _tableRepository.GetByIdAsync(request.TableId);
        if (tableResult.IsError)
            return tableResult.Errors;

        if (tableResult.Value == null)
            return Error.NotFound("Table.NotFound", "Stół nie został znaleziony");

        var table = tableResult.Value;

        // Remove member through aggregate
        var removeMemberResult = table.RemoveMember(request.UserId);
        if (removeMemberResult.IsError)
            return removeMemberResult.Errors;

        // Persist changes (EF change tracking automatically detects changes)
        var saveResult = await _tableRepository.SaveChangesAsync();
        
        if (saveResult.IsError)
            return saveResult.Errors;

        return Result.Success;
    }
}
