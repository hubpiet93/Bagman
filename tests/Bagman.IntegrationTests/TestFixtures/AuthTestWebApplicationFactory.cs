using Bagman.Api;
using Bagman.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bagman.IntegrationTests.TestFixtures;

/// <summary>
/// Custom WebApplicationFactory for Auth integration tests.
/// Configures the web application with a test database.
/// </summary>
public class AuthTestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public AuthTestWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var dbContextDescriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Add test database context with test connection string
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_connectionString);
            });
        });

        // Enable migration and database initialization
        base.ConfigureWebHost(builder);
    }
}
