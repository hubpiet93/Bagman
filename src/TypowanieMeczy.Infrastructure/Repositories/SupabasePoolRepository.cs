using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.Interfaces;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Infrastructure.Services;
using TypowanieMeczy.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Postgrest.Constants;

namespace TypowanieMeczy.Infrastructure.Repositories;

public class SupabasePoolRepository : IPoolRepository
{
    private readonly ISupabaseClient _supabaseClient;
    private readonly ILogger<SupabasePoolRepository> _logger;

    public SupabasePoolRepository(ISupabaseClient supabaseClient, ILogger<SupabasePoolRepository> logger)
    {
        _supabaseClient = supabaseClient;
        _logger = logger;
    }

    public async Task<Pool?> GetByIdAsync(PoolId id)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabasePool>()
                .Select("*")
                .Filter("id", Postgrest.Constants.Operator.Equals, id.ToString())
                .GetAsync();

            var supabasePool = response.FirstOrDefault();
            return supabasePool != null ? MapToDomain(supabasePool) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pool by ID {PoolId}", id);
            return null;
        }
    }

    public async Task<Pool?> GetByMatchIdAsync(MatchId matchId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabasePool>()
                .Select("*")
                .Filter("match_id", Postgrest.Constants.Operator.Equals, matchId.ToString())
                .GetAsync();

            var supabasePool = response.FirstOrDefault();
            return supabasePool != null ? MapToDomain(supabasePool) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pool for match {MatchId}", matchId);
            return null;
        }
    }

    public async Task<IEnumerable<Pool>> GetActivePoolsAsync()
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabasePool>()
                .Select("*")
                .Filter("status", Postgrest.Constants.Operator.Equals, "active")
                .GetAsync();

            return response.Select(MapToDomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active pools");
            return Enumerable.Empty<Pool>();
        }
    }

    public async Task<IEnumerable<Pool>> GetExpiredPoolsAsync()
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabasePool>()
                .Select("*")
                .Filter("status", Postgrest.Constants.Operator.Equals, "expired")
                .GetAsync();

            return response.Select(MapToDomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expired pools");
            return Enumerable.Empty<Pool>();
        }
    }

    public async Task AddAsync(Pool pool)
    {
        try
        {
            var supabasePool = MapToSupabase(pool);
            await _supabaseClient.From<SupabasePool>().Insert(supabasePool);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding pool {PoolId}", pool.Id);
            throw;
        }
    }

    public async Task UpdateAsync(Pool pool)
    {
        try
        {
            var supabasePool = MapToSupabase(pool);
            await _supabaseClient
                .From<SupabasePool>()
                .Filter("id", Postgrest.Constants.Operator.Equals, pool.Id.ToString())
                .Update(supabasePool);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating pool {PoolId}", pool.Id);
            throw;
        }
    }

    public async Task DeleteAsync(PoolId id)
    {
        try
        {
            var pool = await _supabaseClient
                .From<SupabasePool>()
                .Filter("id", Postgrest.Constants.Operator.Equals, id.ToString())
                .GetAsync();
            
            var supabasePool = pool.FirstOrDefault();
            if (supabasePool != null)
            {
                await _supabaseClient.From<SupabasePool>().Delete(supabasePool);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting pool {PoolId}", id);
            throw;
        }
    }

    private Pool MapToDomain(SupabasePool supabasePool)
    {
        return new Pool(MatchId.FromString(supabasePool.MatchId));
    }

    private SupabasePool MapToSupabase(Pool pool)
    {
        return new SupabasePool
        {
            Id = pool.Id.ToString(),
            MatchId = pool.MatchId.ToString(),
            Amount = pool.Amount,
            Status = pool.Status.ToString().ToLower(),
            CreatedAt = pool.CreatedAt
        };
    }
} 