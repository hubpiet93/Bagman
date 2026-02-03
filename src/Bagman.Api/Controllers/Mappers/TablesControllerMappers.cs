using Bagman.Application.Features.Tables.CreateTable;
using Bagman.Application.Features.Tables.GetTableByName;
using Bagman.Application.Features.Tables.GetTableDetails;
using Bagman.Application.Features.Tables.GetUserTables;
using Bagman.Contracts.Models.Tables;

namespace Bagman.Api.Controllers.Mappers;

public static class TablesControllerMappers
{
    public static TableResponse ToTableResponse(this CreateTableResult result)
    {
        return new TableResponse
        {
            Id = result.Id,
            Name = result.Name,
            MaxPlayers = result.MaxPlayers,
            Stake = result.Stake,
            CreatedBy = result.CreatedBy,
            CreatedAt = result.CreatedAt,
            IsSecretMode = result.IsSecretMode
        };
    }

    public static TableResponse ToTableResponse(this TableBasicResult result)
    {
        return new TableResponse
        {
            Id = result.Id,
            Name = result.Name,
            MaxPlayers = result.MaxPlayers,
            Stake = result.Stake,
            CreatedBy = result.CreatedBy,
            CreatedAt = result.CreatedAt,
            IsSecretMode = result.IsSecretMode
        };
    }

    public static TableResponse ToTableResponse(this UserTableResult result)
    {
        return new TableResponse
        {
            Id = result.Id,
            Name = result.Name,
            MaxPlayers = result.MaxPlayers,
            Stake = result.Stake,
            CreatedBy = result.CreatedBy,
            CreatedAt = result.CreatedAt,
            IsSecretMode = result.IsSecretMode
        };
    }

    public static TableDetailResponse ToTableDetailResponse(this TableDetailResult result)
    {
        return new TableDetailResponse
        {
            Id = result.Id,
            Name = result.Name,
            MaxPlayers = result.MaxPlayers,
            Stake = result.Stake,
            CreatedAt = result.CreatedAt,
            Members = result.Members
                .Select(m => m.ToTableMemberResponse())
                .ToList()
        };
    }

    public static TableMemberResponse ToTableMemberResponse(this TableMemberResult result)
    {
        return new TableMemberResponse
        {
            UserId = result.UserId,
            Login = result.Login,
            IsAdmin = result.IsAdmin,
            JoinedAt = result.JoinedAt
        };
    }
}
