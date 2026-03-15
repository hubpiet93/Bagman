using Bagman.Application.Common;
using Bagman.Domain.Repositories;
using ErrorOr;

namespace Bagman.Application.Features.Tables.DeleteTable;

public record DeleteTableCommand
{
    public required Guid TableId { get; init; }
    public required Guid RequestingUserId { get; init; }
}

public class DeleteTableHandler : IFeatureHandler<DeleteTableCommand, Success>
{
    private readonly ITableRepository _tableRepository;

    public DeleteTableHandler(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<ErrorOr<Success>> HandleAsync(
        DeleteTableCommand request,
        CancellationToken cancellationToken = default)
    {
        var tableResult = await _tableRepository.GetByIdAsync(request.TableId);
        if (tableResult.IsError)
            return tableResult.Errors;

        if (tableResult.Value == null)
            return Error.NotFound("Table.NotFound", "Stół nie został znaleziony");

        var table = tableResult.Value;

        if (table.CreatedBy != request.RequestingUserId)
            return Error.Forbidden("Table.NotCreator", "Tylko twórca stołu może go usunąć");

        _tableRepository.Delete(table);

        var saveResult = await _tableRepository.SaveChangesAsync();
        if (saveResult.IsError)
            return saveResult.Errors;

        return Result.Success;
    }
}
