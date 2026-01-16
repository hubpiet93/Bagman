using Testcontainers.PostgreSql;
using Xunit;

namespace Bagman.IntegrationTests.TestFixtures;

/// <summary>
/// Postgres database fixture using TestContainers.
/// Manages lifecycle of a PostgreSQL container for integration tests.
/// </summary>
public class PostgresFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;
    public string? ConnectionString { get; private set; }

    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder()
            .WithDatabase("bagman_test")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .Build();

        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();
    }

    public async Task DisposeAsync()
    {
        if (_container != null)
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }
    }
}
