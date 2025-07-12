using Microsoft.Extensions.Logging;
using Supabase;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using Supabase.Gotrue;
using Supabase.Postgrest;
using Supabase.Postgrest.Interfaces;
using Supabase.Postgrest.Models;
using Microsoft.Extensions.Configuration;
using TypowanieMeczy.Infrastructure.Models;
using Postgrest.Constants;

namespace TypowanieMeczy.Infrastructure.Services;

public class SupabaseClient : ISupabaseClient
{
    private readonly Supabase.Client _supabaseClient;
    private readonly ILogger<SupabaseClient> _logger;

    public SupabaseClient(IConfiguration configuration, ILogger<SupabaseClient> logger)
    {
        var url = configuration["Supabase:Url"];
        var anonKey = configuration["Supabase:AnonKey"];

        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(anonKey))
        {
            throw new InvalidOperationException("Supabase configuration is missing. Please check appsettings.json");
        }

        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        };

        _supabaseClient = new Supabase.Client(url, anonKey, options);
        _logger = logger;
    }

    public ISupabaseAuth Auth => new SupabaseAuthWrapper(_supabaseClient.Auth);

    public ISupabaseTable<T> From<T>() where T : BaseModel, new()
    {
        return new SupabaseTableWrapper<T>(_supabaseClient.From<T>());
    }
}

public class SupabaseAuthWrapper : ISupabaseAuth
{
    private readonly IGotrueClient<User, Session> _auth;

    public SupabaseAuthWrapper(IGotrueClient<User, Session> auth)
    {
        _auth = auth;
    }

    public async Task<ISupabaseAuthResponse> SignUpAsync(string email, string password)
    {
        var response = await _auth.SignUp(email, password);
        return new SupabaseAuthResponseWrapper(response);
    }

    public async Task<ISupabaseAuthResponse> SignInAsync(string email, string password)
    {
        var response = await _auth.SignIn(email, password);
        return new SupabaseAuthResponseWrapper(response);
    }

    public async Task<ISupabaseAuthResponse> SignOutAsync()
    {
        await _auth.SignOut();
        return new SupabaseAuthResponseWrapper(null);
    }

    public async Task<TypowanieMeczy.Infrastructure.Models.SupabaseUser?> GetUserAsync()
    {
        var user = await _auth.GetUser(_auth.CurrentSession?.AccessToken ?? string.Empty);
        return user != null ? new TypowanieMeczy.Infrastructure.Models.SupabaseUser
        {
            Id = user.Id,
            Email = user.Email,
            Login = user.UserMetadata?.GetValueOrDefault("login")?.ToString() ?? string.Empty,
            PasswordHash = string.Empty // Not stored in Supabase Auth
        } : null;
    }
}

public class SupabaseAuthResponseWrapper : ISupabaseAuthResponse
{
    private readonly Session? _session;

    public SupabaseAuthResponseWrapper(Session? session)
    {
        _session = session;
    }

    public TypowanieMeczy.Infrastructure.Models.SupabaseUser? User => _session?.User != null ? new TypowanieMeczy.Infrastructure.Models.SupabaseUser
    {
        Id = _session.User.Id,
        Email = _session.User.Email,
        Login = _session.User.UserMetadata?.GetValueOrDefault("login")?.ToString() ?? string.Empty,
        PasswordHash = string.Empty
    } : null;

    public string? AccessToken => _session?.AccessToken;
    public string? RefreshToken => _session?.RefreshToken;
}

public class SupabaseTableWrapper<T> : ISupabaseTable<T> where T : BaseModel, new()
{
    private readonly IPostgrestTable<T> _table;

    public SupabaseTableWrapper(IPostgrestTable<T> table)
    {
        _table = table;
    }

    public ISupabaseTable<T> Select(string columns)
    {
        _table.Select(columns);
        return this;
    }

    public ISupabaseTable<T> Filter(string column, string op, object value)
    {
        var operatorEnum = op switch
        {
            "eq" => Supabase.Postgrest.Constants.Operator.Equals,
            "gt" => Supabase.Postgrest.Constants.Operator.GreaterThan,
            "lt" => Supabase.Postgrest.Constants.Operator.LessThan,
            "lte" => Supabase.Postgrest.Constants.Operator.LessThanOrEqual,
            "gte" => Supabase.Postgrest.Constants.Operator.GreaterThanOrEqual,
            "in" => Supabase.Postgrest.Constants.Operator.In,
            "like" => Supabase.Postgrest.Constants.Operator.Like,
            "ilike" => Supabase.Postgrest.Constants.Operator.ILike,
            _ => Supabase.Postgrest.Constants.Operator.Equals
        };
        
        _table.Filter(column, operatorEnum, value);
        return this;
    }

    public async Task<List<T>> GetAsync()
    {
        var response = await _table.Get();
        return response.Models;
    }

    public async Task<T> Insert(T item)
    {
        var response = await _table.Insert(item);
        return response.Models.FirstOrDefault() ?? item;
    }

    public async Task<T> Update(T item)
    {
        var response = await _table.Update(item);
        return response.Models.FirstOrDefault() ?? item;
    }

    public async Task Delete(T item)
    {
        await _table.Delete(item);
    }
} 