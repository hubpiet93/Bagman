namespace TypowanieMeczy.Infrastructure.Services;

public class SupabaseClient : ISupabaseClient
{
    public ISupabaseAuth Auth { get; }

    public SupabaseClient(ISupabaseAuth auth)
    {
        Auth = auth;
    }

    public ISupabaseTable<T> From<T>() where T : class
    {
        // Return a stub or mock implementation
        throw new NotImplementedException();
    }
} 