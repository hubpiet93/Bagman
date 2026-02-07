using Bagman.Domain.Repositories;
using Bagman.Domain.Services;
using Bagman.Infrastructure.Repositories;
using Bagman.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bagman.Infrastructure;

public static class AppServicesInitializationExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        // Auth services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuthorizationProvider, AuthorizationProvider>();
        
        // Password hashing
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Generic Repositories (new DDD implementation)
        services.AddScoped<ITableRepository, TableRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IBetRepository, BetRepository>();
        services.AddScoped<IEventTypeRepository, EfEventTypeRepository>();
        
        // Keep old repositories for compatibility during migration
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IPoolRepository, EfPoolRepository>();
        services.AddScoped<IUserStatsRepository, EfUserStatsRepository>();

        return services;
    }
}
