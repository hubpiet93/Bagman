using Bagman.Domain.Common.ValueObjects;
using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using Bagman.Domain.Services;
using Bagman.Application.Common;
using ErrorOr;

namespace Bagman.Application.Features.Tables.CreateTable;

public record CreateTableCommand
{
    public required string Name { get; init; }
    public required string Password { get; init; }
    public required int MaxPlayers { get; init; }
    public required decimal Stake { get; init; }
    public required Guid CreatedBy { get; init; }
    public required Guid EventTypeId { get; init; }
}

public record CreateTableResult
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int MaxPlayers { get; init; }
    public required decimal Stake { get; init; }
    public required Guid CreatedBy { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required bool IsSecretMode { get; init; }
}

public class CreateTableHandler : IFeatureHandler<CreateTableCommand, CreateTableResult>
{
    private readonly ITableRepository _tableRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateTableHandler(
        ITableRepository tableRepository,
        IUserRepository userRepository,
        IEventTypeRepository eventTypeRepository,
        IPasswordHasher passwordHasher)
    {
        _tableRepository = tableRepository;
        _userRepository = userRepository;
        _eventTypeRepository = eventTypeRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ErrorOr<CreateTableResult>> HandleAsync(
        CreateTableCommand request,
        CancellationToken cancellationToken = default)
    {
        // Verify user exists
        var userResult = await _userRepository.GetByIdAsync(request.CreatedBy);
        if (userResult.IsError)
            return userResult.Errors;

        if (userResult.Value == null)
            return Error.NotFound("User.NotFound", "Użytkownik nie został znaleziony");

        // Verify EventType exists and is active
        var eventTypeResult = await _eventTypeRepository.GetByIdAsync(request.EventTypeId);
        if (eventTypeResult.IsError)
            return eventTypeResult.Errors;

        if (eventTypeResult.Value == null)
            return Error.NotFound("EventType.NotFound", "Typ wydarzenia nie został znaleziony");

        if (!eventTypeResult.Value.IsActive)
            return Error.Validation("EventType.NotActive", "Typ wydarzenia nie jest aktywny");

        // Create value objects
        var tableNameResult = TableName.Create(request.Name);
        if (tableNameResult.IsError)
            return tableNameResult.Errors;

        var stakeResult = Money.Create(request.Stake);
        if (stakeResult.IsError)
            return stakeResult.Errors;

        // Hash the table password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Create table aggregate
        var tableResult = Table.Create(
            tableNameResult.Value,
            passwordHash,
            request.MaxPlayers,
            stakeResult.Value,
            request.CreatedBy,
            request.EventTypeId,
            isSecretMode: false);

        if (tableResult.IsError)
            return tableResult.Errors;

        // Persist
        _tableRepository.Add(tableResult.Value);
        var saveResult = await _tableRepository.SaveChangesAsync();
        
        if (saveResult.IsError)
            return saveResult.Errors;

        var table = tableResult.Value;
        return new CreateTableResult
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
