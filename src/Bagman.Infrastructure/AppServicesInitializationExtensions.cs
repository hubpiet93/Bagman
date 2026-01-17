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

        // Repositories
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<ITableRepository, EfTableRepository>();
        services.AddScoped<IMatchRepository, EfMatchRepository>();
        services.AddScoped<IBetRepository, EfBetRepository>();
        services.AddScoped<IPoolRepository, EfPoolRepository>();
        services.AddScoped<IUserStatsRepository, EfUserStatsRepository>();

        // Domain Services
        services.AddScoped<ITableService, TableService>();
        services.AddScoped<IMatchService, MatchService>();
        services.AddScoped<IBetService, BetService>();

        return services;
    }
}
