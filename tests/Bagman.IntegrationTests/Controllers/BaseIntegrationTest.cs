using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using Argon;
using Bagman.Infrastructure.Data;
using Bagman.IntegrationTests.Helpers;
using Bagman.IntegrationTests.TestFixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Engine.ClientProtocol;
using Newtonsoft.Json;
using VerifyTests.Http;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Bagman.IntegrationTests.Controllers;

public abstract class BaseIntegrationTest : AuthTestWebApplicationFactory
{
    protected static readonly Guid DefaultEventTypeId = new Guid("00000000-0000-0000-0000-000000000001");
    protected const string SuperAdminLogin = "test_super_admin";
    protected const string SuperAdminPassword = "SuperAdmin@123";
    
    protected string? CurrentTestName { get; }
    private readonly PostgresFixture _postgresFixture;
    private readonly VerifySettings _verifySettings = new();

    private bool _initialized;

    protected HttpClient HttpClient { get; private set; }
    protected RecordingHandler? RecordingHandler { get; private set; }

    protected BaseIntegrationTest(PostgresFixture postgresFixture, ITestOutputHelper output) : base(postgresFixture.ConnectionString!)
    {
        _postgresFixture = postgresFixture;
        CurrentTestName = ExtractTestDisplayName(output);
    }
    
    private static string? ExtractTestDisplayName(ITestOutputHelper output)
    {
        var testField = output.GetType()
            .GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);

        var test = testField?.GetValue(output);

        var testCaseProperty = test?.GetType()
            .GetProperty("TestCase", BindingFlags.Instance | BindingFlags.Public);

        var testCase = testCaseProperty?.GetValue(test);

        var displayNameProperty = testCase?.GetType()
            .GetProperty("DisplayName", BindingFlags.Instance | BindingFlags.Public);
        return displayNameProperty?.GetValue(testCase) as string;
    }

    protected async Task Init()
    {
        if (_initialized)
            return;

        _initialized = true;

        _verifySettings.AddExtraSettings(options =>
        {
            options.DefaultValueHandling = Argon.DefaultValueHandling.Include;
            options.NullValueHandling = Argon.NullValueHandling.Include;
            options.Formatting = Argon.Formatting.Indented;
            options.Converters.Add(new RecordedHttpMessageConverter());
        });

        await InitializeHttpClient();
        await InitializeDatabase();
    }

    protected async Task VerifyHttpRecording()
    {
        var sends = new List<object>();
        
        sends.Add(new 
        {
            TestName = CurrentTestName
        });
        
        sends.AddRange(RecordingHandler.Sends);
        
        await Verify(sends, _verifySettings)
            .UseStrictJson();
    }

    protected async Task Dispose()
    {
        HttpClient.Dispose();
        RecordingHandler?.Dispose();

        await base.DisposeAsync();
    }

    private Task InitializeHttpClient()
    {
        RecordingHandler = new RecordingHandler();
        HttpClient = CreateDefaultClient(RecordingHandler);
        return Task.CompletedTask;
    }

    private async Task InitializeDatabase()
    {
        if (_postgresFixture.ConnectionString == null)
            await _postgresFixture.InitializeAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        const int maxRetries = 3;
        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                await dbContext.Database.EnsureCreatedAsync();
                await dbContext.Database.ExecuteSqlRawAsync(
                    "TRUNCATE TABLE pool_winners, pools, bets, matches, user_stats, table_members, tables, refresh_tokens, users, event_types RESTART IDENTITY CASCADE;");
                
                // Create default EventType for tests
                await dbContext.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO event_types (id, code, name, start_date, is_active, created_at)
                    VALUES ('00000000-0000-0000-0000-000000000001'::uuid, 'TEST_DEFAULT', 'Default Test Event', NOW() - INTERVAL '1 day', TRUE, NOW());
                ");
                
                break;
            }
            catch (Exception) when (i < maxRetries - 1)
            {
                await Task.Delay(1000);
            }
        }
    }
}
