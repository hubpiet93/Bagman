using TypowanieMeczy.Domain.Common;
using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.Interfaces;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Infrastructure.Services;
using TypowanieMeczy.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Postgrest.Constants;

namespace TypowanieMeczy.Infrastructure.Repositories;

public class SupabaseMatchRepository : IMatchRepository
{
    private readonly ISupabaseClient _supabaseClient;
    private readonly ILogger<SupabaseMatchRepository> _logger;

    public SupabaseMatchRepository(ISupabaseClient supabaseClient, ILogger<SupabaseMatchRepository> logger)
    {
        _supabaseClient = supabaseClient;
        _logger = logger;
    }

    public async Task<Match?> GetByIdAsync(MatchId id)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseMatch>()
                .Select("*")
                .Filter("id", Postgrest.Constants.Operator.Equals, id.ToString())
                .GetAsync();

            var supabaseMatch = response.FirstOrDefault();
            return supabaseMatch != null ? MapToDomain(supabaseMatch) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting match by ID {MatchId}", id);
            return null;
        }
    }

    public async Task<IEnumerable<Match>> GetByTableIdAsync(TableId tableId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseMatch>()
                .Select("*")
                .Filter("table_id", Postgrest.Constants.Operator.Equals, tableId.ToString())
                .GetAsync();

            return response.Select(MapToDomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting matches for table {TableId}", tableId);
            return Enumerable.Empty<Match>();
        }
    }

    public async Task<IEnumerable<Match>> GetUpcomingByTableIdAsync(TableId tableId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseMatch>()
                .Select("*")
                .Filter("table_id", Postgrest.Constants.Operator.Equals, tableId.ToString())
                .Filter("match_datetime", Postgrest.Constants.Operator.GreaterThan, DateTime.UtcNow.ToString("O"))
                .GetAsync();

            return response.Select(MapToDomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upcoming matches for table {TableId}", tableId);
            return Enumerable.Empty<Match>();
        }
    }

    public async Task<IEnumerable<Match>> GetFinishedByTableIdAsync(TableId tableId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseMatch>()
                .Select("*")
                .Filter("table_id", Postgrest.Constants.Operator.Equals, tableId.ToString())
                .Filter("status", Postgrest.Constants.Operator.Equals, "finished")
                .GetAsync();

            return response.Select(MapToDomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting finished matches for table {TableId}", tableId);
            return Enumerable.Empty<Match>();
        }
    }

    public async Task<IEnumerable<Match>> GetByTableIdAndStatusAsync(TableId tableId, MatchStatus status)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseMatch>()
                .Select("*")
                .Filter("table_id", Postgrest.Constants.Operator.Equals, tableId.ToString())
                .Filter("status", Postgrest.Constants.Operator.Equals, status.ToString().ToLower())
                .GetAsync();

            return response.Select(MapToDomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting matches for table {TableId} with status {Status}", tableId, status);
            return Enumerable.Empty<Match>();
        }
    }

    public async Task AddAsync(Match match)
    {
        try
        {
            var supabaseMatch = MapToSupabase(match);
            await _supabaseClient.From<SupabaseMatch>().Insert(supabaseMatch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding match {MatchId}", match.Id);
            throw;
        }
    }

    public async Task UpdateAsync(Match match)
    {
        try
        {
            var supabaseMatch = MapToSupabase(match);
            await _supabaseClient
                .From<SupabaseMatch>()
                .Filter("id", Postgrest.Constants.Operator.Equals, match.Id.ToString())
                .Update(supabaseMatch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating match {MatchId}", match.Id);
            throw;
        }
    }

    public async Task DeleteAsync(MatchId id)
    {
        try
        {
            var match = await _supabaseClient
                .From<SupabaseMatch>()
                .Filter("id", Postgrest.Constants.Operator.Equals, id.ToString())
                .GetAsync();
            
            var supabaseMatch = match.FirstOrDefault();
            if (supabaseMatch != null)
            {
                await _supabaseClient.From<SupabaseMatch>().Delete(supabaseMatch);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting match {MatchId}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(MatchId id)
    {
        var match = await GetByIdAsync(id);
        return match != null;
    }

    public async Task<IEnumerable<Match>> GetMatchesNeedingStartAsync()
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseMatch>()
                .Select("*")
                .Filter("status", Postgrest.Constants.Operator.Equals, "scheduled")
                .Filter("match_datetime", Postgrest.Constants.Operator.LessThanOrEqual, DateTime.UtcNow.ToString("O"))
                .GetAsync();

            return response.Select(MapToDomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting matches needing start");
            return Enumerable.Empty<Match>();
        }
    }

    public async Task<IEnumerable<Match>> GetMatchesNeedingFinishAsync()
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseMatch>()
                .Select("*")
                .Filter("status", Postgrest.Constants.Operator.Equals, "in_progress")
                .Filter("match_datetime", Postgrest.Constants.Operator.LessThan, DateTime.UtcNow.AddHours(-2).ToString("O"))
                .GetAsync();

            return response.Select(MapToDomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting matches needing finish");
            return Enumerable.Empty<Match>();
        }
    }

    private Match MapToDomain(SupabaseMatch supabaseMatch)
    {
        return new Match(
            TableId.FromString(supabaseMatch.TableId),
            new Country(supabaseMatch.Country1),
            new Country(supabaseMatch.Country2),
            new MatchDateTime(supabaseMatch.MatchDateTime),
            UserId.FromString(supabaseMatch.CreatedBy)
        );
    }

    private SupabaseMatch MapToSupabase(Match match)
    {
        return new SupabaseMatch
        {
            Id = match.Id.ToString(),
            TableId = match.TableId.ToString(),
            Country1 = match.Country1.Value,
            Country2 = match.Country2.Value,
            MatchDateTime = match.MatchDateTime.Value,
            Result = match.Result?.Value,
            Status = match.Status.ToString().ToLower(),
            Started = match.IsStarted,
            CreatedBy = match.CreatedBy.ToString(),
            CreatedAt = match.CreatedAt
        };
    }
} 