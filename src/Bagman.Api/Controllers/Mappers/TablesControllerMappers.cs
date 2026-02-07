using Bagman.Application.Features.Tables.CreateTable;
using Bagman.Application.Features.Tables.GetTableByName;
using Bagman.Application.Features.Tables.GetTableDetails;
using Bagman.Application.Features.Tables.GetTableDashboard;
using Bagman.Application.Features.Tables.GetUserTables;
using Bagman.Contracts.Models;
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
                .OrderBy(m => m.JoinedAt)
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

    public static TableDashboardResponse ToTableDashboardResponse(this TableDashboardResult result)
    {
        return new TableDashboardResponse
        {
            Table = new TableInfo
            {
                Id = result.TableId,
                Name = result.TableName,
                MaxPlayers = result.MaxPlayers,
                Stake = result.Stake,
                CreatedAt = result.TableCreatedAt
            },
            Members = result.Members
                .Select(m => m.ToMemberInfo())
                .ToList(),
            Matches = result.Matches
                .Select(m => m.ToMatchInfo())
                .ToList(),
            Bets = result.Bets
                .Select(b => b.ToBetInfo())
                .ToList(),
            Pools = result.Pools
                .Select(p => p.ToPoolInfo())
                .ToList(),
            Stats = result.Stats
                .Select(s => s.ToStatsInfo())
                .ToList()
        };
    }

    public static MemberInfo ToMemberInfo(this MemberDetailResult result)
    {
        return new MemberInfo
        {
            UserId = result.UserId,
            Login = result.Login,
            IsAdmin = result.IsAdmin,
            JoinedAt = result.JoinedAt
        };
    }

    public static MatchInfo ToMatchInfo(this MatchDetailResult result)
    {
        return new MatchInfo
        {
            Id = result.Id,
            Country1 = result.Country1,
            Country2 = result.Country2,
            MatchDateTime = result.MatchDateTime,
            Result = result.Result,
            IsStarted = result.IsStarted
        };
    }

    public static BetInfo ToBetInfo(this BetDetailResult result)
    {
        return new BetInfo
        {
            Id = result.Id,
            UserId = result.UserId,
            MatchId = result.MatchId,
            Prediction = result.Prediction,
            EditedAt = result.EditedAt
        };
    }

    public static PoolInfo ToPoolInfo(this PoolDetailResult result)
    {
        return new PoolInfo
        {
            Id = result.Id,
            MatchId = result.MatchId,
            Amount = result.Amount,
            Status = result.Status,
            Winners = result.Winners
        };
    }

    public static StatsInfo ToStatsInfo(this StatsDetailResult result)
    {
        return new StatsInfo
        {
            UserId = result.UserId,
            MatchesPlayed = result.MatchesPlayed,
            BetsPlaced = result.BetsPlaced,
            PoolsWon = result.PoolsWon,
            TotalWon = result.TotalWon
        };
    }
}
