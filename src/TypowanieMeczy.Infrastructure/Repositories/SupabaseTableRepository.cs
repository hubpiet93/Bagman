using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.Interfaces;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Infrastructure.Services;
using TypowanieMeczy.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Postgrest.Constants;

namespace TypowanieMeczy.Infrastructure.Repositories;

public class SupabaseTableRepository : ITableRepository
{
    private readonly ISupabaseClient _supabaseClient;
    private readonly ILogger<SupabaseTableRepository> _logger;

    public SupabaseTableRepository(ISupabaseClient supabaseClient, ILogger<SupabaseTableRepository> logger)
    {
        _supabaseClient = supabaseClient;
        _logger = logger;
    }

    public async Task<Table?> GetByIdAsync(TableId id)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseTable>()
                .Select("*")
                .Filter("id", Postgrest.Constants.Operator.Equals, id.ToString())
                .GetAsync();

            var supabaseTable = response.FirstOrDefault();
            return supabaseTable != null ? MapToDomain(supabaseTable) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting table by ID {TableId}", id);
            return null;
        }
    }

    public async Task<Table?> GetByNameAsync(TableName name)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseTable>()
                .Select("*")
                .Filter("name", Postgrest.Constants.Operator.Equals, name.Value)
                .GetAsync();

            var supabaseTable = response.FirstOrDefault();
            return supabaseTable != null ? MapToDomain(supabaseTable) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting table by name {TableName}", name.Value);
            return null;
        }
    }

    public async Task<IEnumerable<Table>> GetByUserIdAsync(UserId userId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseTableMembership>()
                .Select("table_id")
                .Filter("user_id", Postgrest.Constants.Operator.Equals, userId.ToString())
                .GetAsync();

            var tableIds = response.Select(tm => tm.TableId).ToList();
            var tables = new List<Table>();

            foreach (var tableId in tableIds)
            {
                var table = await GetByIdAsync(TableId.FromString(tableId));
                if (table != null)
                    tables.Add(table);
            }

            return tables;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tables for user {UserId}", userId);
            return Enumerable.Empty<Table>();
        }
    }

    public async Task<IEnumerable<Table>> GetAllAsync()
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseTable>()
                .Select("*")
                .GetAsync();

            return response.Select(MapToDomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tables");
            return Enumerable.Empty<Table>();
        }
    }

    public async Task<bool> ExistsAsync(TableId id)
    {
        var table = await GetByIdAsync(id);
        return table != null;
    }

    public async Task<bool> ExistsByNameAsync(TableName name)
    {
        var table = await GetByNameAsync(name);
        return table != null;
    }

    public async Task AddAsync(Table table)
    {
        try
        {
            var supabaseTable = MapToSupabase(table);
            await _supabaseClient.From<SupabaseTable>().Insert(supabaseTable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding table {TableId}", table.Id);
            throw;
        }
    }

    public async Task UpdateAsync(Table table)
    {
        try
        {
            var supabaseTable = MapToSupabase(table);
            await _supabaseClient
                .From<SupabaseTable>()
                .Filter("id", Postgrest.Constants.Operator.Equals, table.Id.ToString())
                .Update(supabaseTable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating table {TableId}", table.Id);
            throw;
        }
    }

    public async Task DeleteAsync(TableId id)
    {
        try
        {
            var table = await _supabaseClient
                .From<SupabaseTable>()
                .Filter("id", Postgrest.Constants.Operator.Equals, id.ToString())
                .GetAsync();
            
            var supabaseTable = table.FirstOrDefault();
            if (supabaseTable != null)
            {
                await _supabaseClient.From<SupabaseTable>().Delete(supabaseTable);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting table {TableId}", id);
            throw;
        }
    }

    public async Task<bool> IsUserMemberAsync(TableId tableId, UserId userId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseTableMembership>()
                .Select("*")
                .Filter("table_id", Postgrest.Constants.Operator.Equals, tableId.ToString())
                .Filter("user_id", Postgrest.Constants.Operator.Equals, userId.ToString())
                .GetAsync();

            return response.Any();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} is member of table {TableId}", userId, tableId);
            return false;
        }
    }

    public async Task<bool> IsUserAdminAsync(TableId tableId, UserId userId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseTableMembership>()
                .Select("*")
                .Filter("table_id", Postgrest.Constants.Operator.Equals, tableId.ToString())
                .Filter("user_id", Postgrest.Constants.Operator.Equals, userId.ToString())
                .Filter("is_admin", Postgrest.Constants.Operator.Equals, true)
                .GetAsync();

            return response.Any();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} is admin of table {TableId}", userId, tableId);
            return false;
        }
    }

    public async Task<int> GetMemberCountAsync(TableId tableId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseTableMembership>()
                .Select("*")
                .Filter("table_id", Postgrest.Constants.Operator.Equals, tableId.ToString())
                .GetAsync();

            return response.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting member count for table {TableId}", tableId);
            return 0;
        }
    }

    public async Task<IEnumerable<TableMembership>> GetMembershipsAsync(TableId tableId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseTableMembership>()
                .Select("*")
                .Filter("table_id", Postgrest.Constants.Operator.Equals, tableId.ToString())
                .GetAsync();

            return response.Select(MapToDomainMembership);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting memberships for table {TableId}", tableId);
            return Enumerable.Empty<TableMembership>();
        }
    }

    private Table MapToDomain(SupabaseTable supabaseTable)
    {
        return new Table(
            new TableName(supabaseTable.Name),
            new PasswordHash(supabaseTable.PasswordHash),
            new MaxPlayers(supabaseTable.MaxPlayers),
            new Stake(supabaseTable.Stake),
            UserId.FromString(supabaseTable.CreatedBy)
        );
    }

    private SupabaseTable MapToSupabase(Table table)
    {
        return new SupabaseTable
        {
            Id = table.Id.ToString(),
            Name = table.Name.Value,
            PasswordHash = table.PasswordHash.Value,
            MaxPlayers = table.MaxPlayers.Value,
            Stake = table.Stake.Value,
            CreatedBy = table.CreatedBy.ToString(),
            CreatedAt = table.CreatedAt,
            IsSecretMode = table.IsSecretMode
        };
    }

    private TableMembership MapToDomainMembership(SupabaseTableMembership supabaseMembership)
    {
        return new TableMembership(
            UserId.FromString(supabaseMembership.UserId),
            TableId.FromString(supabaseMembership.TableId),
            supabaseMembership.IsAdmin
        );
    }
} 