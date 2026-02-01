using System.Reflection;
using Argon;
using Bagman.Infrastructure.Data;
using Bagman.IntegrationTests.Helpers;
using Bagman.IntegrationTests.TestFixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Engine.ClientProtocol;
using VerifyTests.Http;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Bagman.IntegrationTests.Controllers;

public abstract class BaseIntegrationTest : AuthTestWebApplicationFactory
{
    protected string CurrentTestName { get; }
    private readonly PostgresFixture _postgresFixture;
    private readonly VerifySettings _verifySettings = new();

    private bool _initialized;

    private PostgresFixture PostgresFixture
        => Services.GetService<PostgresFixture>()
           ?? throw new InvalidOperationException("PostgresFixture not available from factory services.");

    protected HttpClient HttpClient { get; private set; }
    protected RecordingHandler RecordingHandler { get; private set; }

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
            options.DefaultValueHandling = DefaultValueHandling.Include;
            options.NullValueHandling = NullValueHandling.Include;
            options.Formatting = Formatting.Indented;
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
        RecordingHandler.Dispose();

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
            try
            {
                await dbContext.Database.EnsureCreatedAsync();
                await dbContext.Database.ExecuteSqlRawAsync(
                    "TRUNCATE TABLE pool_winners, pools, bets, matches, user_stats, table_members, tables, refresh_tokens, users RESTART IDENTITY CASCADE;");
                break;
            }
            catch (Exception) when (i < maxRetries - 1)
            {
                await Task.Delay(1000);
            }
    }
}
