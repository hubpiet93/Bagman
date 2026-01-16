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
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuthorizationProvider, AuthorizationProvider>();
        services.AddScoped<IUserRepository, EfUserRepository>();

        return services;
    }
}
