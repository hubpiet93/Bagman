using Supabase.Postgrest.Models;
using TypowanieMeczy.Infrastructure.Models;

namespace TypowanieMeczy.Infrastructure.Services;

public interface ISupabaseClient
{
    ISupabaseAuth Auth { get; }
    ISupabaseTable<T> From<T>() where T : BaseModel, new();
}

public interface ISupabaseAuth
{
    Task<ISupabaseAuthResponse> SignUpAsync(string email, string password);
    Task<ISupabaseAuthResponse> SignInAsync(string email, string password);
    Task<ISupabaseAuthResponse> SignOutAsync();
    Task<TypowanieMeczy.Infrastructure.Models.SupabaseUser?> GetUserAsync();
}

public interface ISupabaseAuthResponse
{
    TypowanieMeczy.Infrastructure.Models.SupabaseUser? User { get; }
    string? AccessToken { get; }
    string? RefreshToken { get; }
}

public interface ISupabaseTable<T> where T : BaseModel, new()
{
    ISupabaseTable<T> Select(string columns);
    ISupabaseTable<T> Filter(string column, string op, object value);
    Task<List<T>> GetAsync();
    Task<T> Insert(T item);
    Task<T> Update(T item);
    Task Delete(T item);
} 