using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.Interfaces;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Infrastructure.Services;
using TypowanieMeczy.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Postgrest.Constants;

namespace TypowanieMeczy.Infrastructure.Repositories;

public class SupabaseUserRepository : IUserRepository
{
    private readonly ISupabaseClient _supabaseClient;
    private readonly ILogger<SupabaseUserRepository> _logger;

    public SupabaseUserRepository(ISupabaseClient supabaseClient, ILogger<SupabaseUserRepository> logger)
    {
        _supabaseClient = supabaseClient;
        _logger = logger;
    }

    public async Task<User?> GetByIdAsync(UserId id)
    {
        try
        {
            var response = await _supabaseClient
                .From<Models.SupabaseUser>()
                .Select("*")
                .Filter("id", Postgrest.Constants.Operator.Equals, id.ToString())
                .GetAsync();

            var supabaseUser = response.FirstOrDefault();
            return supabaseUser != null ? MapToDomain(supabaseUser) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID {UserId}", id);
            return null;
        }
    }

    public async Task<User?> GetByLoginAsync(Login login)
    {
        try
        {
            var response = await _supabaseClient
                .From<Models.SupabaseUser>()
                .Select("*")
                .Filter("login", Postgrest.Constants.Operator.Equals, login.Value)
                .GetAsync();

            var supabaseUser = response.FirstOrDefault();
            return supabaseUser != null ? MapToDomain(supabaseUser) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by login {Login}", login.Value);
            return null;
        }
    }

    public async Task<User?> GetByEmailAsync(Email email)
    {
        try
        {
            var response = await _supabaseClient
                .From<Models.SupabaseUser>()
                .Select("*")
                .Filter("email", Postgrest.Constants.Operator.Equals, email.Value)
                .GetAsync();

            var supabaseUser = response.FirstOrDefault();
            return supabaseUser != null ? MapToDomain(supabaseUser) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email {Email}", email.Value);
            return null;
        }
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        try
        {
            var response = await _supabaseClient
                .From<Models.SupabaseUser>()
                .Select("*")
                .GetAsync();

            return response.Select(MapToDomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            return Enumerable.Empty<User>();
        }
    }

    public async Task<bool> ExistsAsync(UserId id)
    {
        var user = await GetByIdAsync(id);
        return user != null;
    }

    public async Task<bool> ExistsByLoginAsync(Login login)
    {
        var user = await GetByLoginAsync(login);
        return user != null;
    }

    public async Task<bool> ExistsByEmailAsync(Email email)
    {
        var user = await GetByEmailAsync(email);
        return user != null;
    }

    public async Task AddAsync(User user)
    {
        try
        {
            var supabaseUser = MapToSupabase(user);
            await _supabaseClient.From<Models.SupabaseUser>().Insert(supabaseUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user {UserId}", user.Id);
            throw;
        }
    }

    public async Task UpdateAsync(User user)
    {
        try
        {
            var supabaseUser = MapToSupabase(user);
            await _supabaseClient
                .From<Models.SupabaseUser>()
                .Filter("id", Postgrest.Constants.Operator.Equals, user.Id.ToString())
                .Update(supabaseUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", user.Id);
            throw;
        }
    }

    public async Task DeleteAsync(UserId id)
    {
        try
        {
            var user = await _supabaseClient
                .From<Models.SupabaseUser>()
                .Filter("id", Postgrest.Constants.Operator.Equals, id.ToString())
                .GetAsync();
            
            var supabaseUser = user.FirstOrDefault();
            if (supabaseUser != null)
            {
                await _supabaseClient.From<Models.SupabaseUser>().Delete(supabaseUser);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<User>> GetByTableIdAsync(TableId tableId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseTableMembership>()
                .Select("user_id")
                .Filter("table_id", Postgrest.Constants.Operator.Equals, tableId.ToString())
                .GetAsync();

            var userIds = response.Select(tm => tm.UserId).ToList();
            var users = new List<User>();

            foreach (var userId in userIds)
            {
                var user = await GetByIdAsync(UserId.FromString(userId));
                if (user != null)
                    users.Add(user);
            }

            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users for table {TableId}", tableId);
            return Enumerable.Empty<User>();
        }
    }

    public async Task<IEnumerable<User>> GetAdminsByTableIdAsync(TableId tableId)
    {
        try
        {
            var response = await _supabaseClient
                .From<SupabaseTableMembership>()
                .Select("user_id")
                .Filter("table_id", Postgrest.Constants.Operator.Equals, tableId.ToString())
                .Filter("is_admin", Postgrest.Constants.Operator.Equals, true)
                .GetAsync();

            var userIds = response.Select(tm => tm.UserId).ToList();
            var users = new List<User>();

            foreach (var userId in userIds)
            {
                var user = await GetByIdAsync(UserId.FromString(userId));
                if (user != null)
                    users.Add(user);
            }

            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting admins for table {TableId}", tableId);
            return Enumerable.Empty<User>();
        }
    }

    private User MapToDomain(Models.SupabaseUser supabaseUser)
    {
        return new User(
            new Login(supabaseUser.Login),
            new Email(supabaseUser.Email ?? string.Empty),
            new PasswordHash(supabaseUser.PasswordHash)
        );
    }

    private Models.SupabaseUser MapToSupabase(User user)
    {
        return new Models.SupabaseUser
        {
            Id = user.Id.ToString(),
            Login = user.Login.Value,
            Email = user.Email.Value,
            PasswordHash = user.PasswordHash.Value,
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive
        };
    }
} 