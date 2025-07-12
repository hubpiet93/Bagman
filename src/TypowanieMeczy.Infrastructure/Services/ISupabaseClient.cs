namespace TypowanieMeczy.Infrastructure.Services;

public interface ISupabaseClient
{
    ISupabaseAuth Auth { get; }
    ISupabaseTable<T> From<T>() where T : class;
}

public interface ISupabaseAuth
{
    Task<ISupabaseAuthResponse> SignUpAsync(string email, string password);
    Task<ISupabaseAuthResponse> SignInAsync(string email, string password);
    Task<ISupabaseAuthResponse> SignOutAsync();
    Task<SupabaseUser?> GetUserAsync();
}

public interface ISupabaseAuthResponse
{
    SupabaseUser? User { get; }
    string? AccessToken { get; }
    string? RefreshToken { get; }
}

public interface ISupabaseTable<T> where T : class
{
    ISupabaseTable<T> Select(string columns);
    ISupabaseTable<T> Filter(string column, string op, object value);
    Task<List<T>> GetAsync();
} 