using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TypowanieMeczy.Domain.Services;
using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Infrastructure.Services;

public class MatchManagementBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MatchManagementBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public MatchManagementBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<MatchManagementBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Match Management Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessMatchesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing matches in background service");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Match Management Background Service stopped");
    }

    private async Task ProcessMatchesAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var matchService = scope.ServiceProvider.GetRequiredService<IMatchService>();

        // Process matches that need to start
        var matchesToStart = await matchService.GetMatchesNeedingStartAsync();
        foreach (var match in matchesToStart)
        {
            try
            {
                if (await matchService.CanStartMatchAsync(match.Id))
                {
                    await matchService.StartMatchAsync(match.Id);
                    _logger.LogInformation("Started match {MatchId}", match.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting match {MatchId}", match.Id);
            }
        }

        // Process matches that need to finish
        var matchesToFinish = await matchService.GetMatchesNeedingFinishAsync();
        foreach (var match in matchesToFinish)
        {
            try
            {
                if (await matchService.CanFinishMatchAsync(match.Id))
                {
                    // In a real scenario, you would get the actual result from an external API
                    // For now, we'll use a placeholder result
                    var placeholderResult = new MatchResult("1:0"); // Placeholder
                    await matchService.FinishMatchAsync(match.Id, placeholderResult);
                    _logger.LogInformation("Finished match {MatchId} with result {Result}", match.Id, placeholderResult.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finishing match {MatchId}", match.Id);
            }
        }
    }
} 