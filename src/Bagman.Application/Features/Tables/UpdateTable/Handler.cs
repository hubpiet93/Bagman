using Bagman.Application.Common;
using Bagman.Domain.Common.ValueObjects;
using Bagman.Domain.Repositories;
using Bagman.Domain.Services;
using ErrorOr;

namespace Bagman.Application.Features.Tables.UpdateTable;

public record UpdateTableCommand
{
    public required Guid TableId { get; init; }
    public required Guid RequestingUserId { get; init; }
    public string? Name { get; init; }
    public string? Password { get; init; }
    public int? MaxPlayers { get; init; }
    public decimal? Stake { get; init; }
}

public record UpdateTableResult
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int MaxPlayers { get; init; }
    public required decimal Stake { get; init; }
    public required Guid CreatedBy { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required bool IsSecretMode { get; init; }
}

public class UpdateTableHandler : IFeatureHandler<UpdateTableCommand, UpdateTableResult>
{
    private readonly ITableRepository _tableRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateTableHandler(ITableRepository tableRepository, IPasswordHasher passwordHasher)
    {
        _tableRepository = tableRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ErrorOr<UpdateTableResult>> HandleAsync(
        UpdateTableCommand request,
        CancellationToken cancellationToken = default)
    {
        var tableResult = await _tableRepository.GetByIdAsync(request.TableId);
        if (tableResult.IsError)
            return tableResult.Errors;

        if (tableResult.Value == null)
            return Error.NotFound("Table.NotFound", "Stół nie został znaleziony");

        var table = tableResult.Value;

        if (!table.IsUserAdmin(request.RequestingUserId))
            return Error.Forbidden("Table.NotAdmin", "Nie masz uprawnień do edycji tego stołu");

        TableName? newName = null;
        if (request.Name is not null)
        {
            var nameResult = TableName.Create(request.Name);
            if (nameResult.IsError)
                return nameResult.Errors;
            newName = nameResult.Value;
        }

        Money? newStake = null;
        if (request.Stake.HasValue)
        {
            var stakeResult = Money.Create(request.Stake.Value);
            if (stakeResult.IsError)
                return stakeResult.Errors;
            newStake = stakeResult.Value;
        }

        string? newPasswordHash = request.Password is not null
            ? _passwordHasher.HashPassword(request.Password)
            : null;

        var updateResult = table.Update(newName, newPasswordHash, request.MaxPlayers, newStake);
        if (updateResult.IsError)
            return updateResult.Errors;

        var saveResult = await _tableRepository.SaveChangesAsync();
        if (saveResult.IsError)
            return saveResult.Errors;

        return new UpdateTableResult
        {
            Id = table.Id,
            Name = table.Name.Value,
            MaxPlayers = table.MaxPlayers,
            Stake = table.Stake.Amount,
            CreatedBy = table.CreatedBy,
            CreatedAt = table.CreatedAt,
            IsSecretMode = table.IsSecretMode
        };
    }
}
