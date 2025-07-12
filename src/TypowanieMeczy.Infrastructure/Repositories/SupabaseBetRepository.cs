using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.Interfaces;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Infrastructure.Services;
using TypowanieMeczy.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Postgrest.Constants;

namespace TypowanieMeczy.Infrastructure.Repositories;

public class SupabaseBetRepository : IBetRepository
{
    private readonly ISupabaseClient _supabaseClient;
    private readonly ILogger<SupabaseBetRepository> _logger;

    public SupabaseBetRepository(ISupabaseClient supabaseClient, ILogger<SupabaseBetRepository> logger)
    {
        _supabaseClient = supabaseClient;
        _logger = logger;
    }

    public async Task<Bet?> GetByIdAsync(BetId id)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseBet>()
                .Select("*")
                .Filter("id", Postgrest.Constants.Operator.Equals, id.ToString())
                .GetAsync();

            var supabaseBet = response.FirstOrDefault();
            return supabaseBet != null ? MapToDomain(supabaseBet) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bet by ID {BetId}", id);
            return null;
        }
    }

    public async Task<IEnumerable<Bet>> GetByMatchIdAsync(MatchId matchId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseBet>()
                .Select("*")
                .Filter("match_id", Postgrest.Constants.Operator.Equals, matchId.ToString())
                .GetAsync();

            return response.Select(MapToDomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bets for match {MatchId}", matchId);
            return Enumerable.Empty<Bet>();
        }
    }

    public async Task<IEnumerable<Bet>> GetByUserIdAsync(UserId userId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseBet>()
                .Select("*")
                .Filter("user_id", Postgrest.Constants.Operator.Equals, userId.ToString())
                .GetAsync();

            return response.Select(MapToDomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bets for user {UserId}", userId);
            return Enumerable.Empty<Bet>();
        }
    }

    public async Task<Bet?> GetByUserAndMatchAsync(UserId userId, MatchId matchId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseBet>()
                .Select("*")
                .Filter("user_id", Postgrest.Constants.Operator.Equals, userId.ToString())
                .Filter("match_id", Postgrest.Constants.Operator.Equals, matchId.ToString())
                .GetAsync();

            var supabaseBet = response.FirstOrDefault();
            return supabaseBet != null ? MapToDomain(supabaseBet) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bet for user {UserId} and match {MatchId}", userId, matchId);
            return null;
        }
    }

    public async Task<IEnumerable<Bet>> GetWinnersByMatchIdAsync(MatchId matchId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseBet>()
                .Select("*")
                .Filter("match_id", Postgrest.Constants.Operator.Equals, matchId.ToString())
                .Filter("is_winner", Postgrest.Constants.Operator.Equals, true)
                .GetAsync();

            return response.Select(MapToDomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting winners for match {MatchId}", matchId);
            return Enumerable.Empty<Bet>();
        }
    }

    public async Task AddAsync(Bet bet)
    {
        try
        {
            var supabaseBet = MapToSupabase(bet);
            await _supabaseClient.From<SupabaseBet>().Insert(supabaseBet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding bet {BetId}", bet.Id);
            throw;
        }
    }

    public async Task UpdateAsync(Bet bet)
    {
        try
        {
            var supabaseBet = MapToSupabase(bet);
            await _supabaseClient
                .From<SupabaseBet>()
                .Filter("id", Postgrest.Constants.Operator.Equals, bet.Id.ToString())
                .Update(supabaseBet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating bet {BetId}", bet.Id);
            throw;
        }
    }

    public async Task DeleteAsync(BetId id)
    {
        try
        {
            var bet = await _supabaseClient
                .From<SupabaseBet>()
                .Filter("id", Postgrest.Constants.Operator.Equals, id.ToString())
                .GetAsync();
            
            var supabaseBet = bet.FirstOrDefault();
            if (supabaseBet != null)
            {
                await _supabaseClient.From<SupabaseBet>().Delete(supabaseBet);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting bet {BetId}", id);
            throw;
        }
    }

    private Bet MapToDomain(SupabaseBet supabaseBet)
    {
        return new Bet(
            UserId.FromString(supabaseBet.UserId),
            MatchId.FromString(supabaseBet.MatchId),
            new MatchPrediction(supabaseBet.Prediction)
        );
    }

    private SupabaseBet MapToSupabase(Bet bet)
    {
        return new SupabaseBet
        {
            Id = bet.Id.ToString(),
            UserId = bet.UserId.ToString(),
            MatchId = bet.MatchId.ToString(),
            Prediction = bet.Prediction.Value,
            EditedAt = bet.EditedAt,
            IsWinner = bet.IsWinner
        };
    }
} 