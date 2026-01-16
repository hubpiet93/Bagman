using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bagman.Infrastructure.Data;

public static class ApplicationDbContextInitializationExtensions
{
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Database connection string 'Postgres' is missing. Please check appsettings.json or secrets.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}
