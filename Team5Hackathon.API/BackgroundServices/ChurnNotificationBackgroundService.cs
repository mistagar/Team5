using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Team5Hackathon.Application.Services;
using Team5Hackathon.Domain.RepositoriesContract;

namespace Team5Hackathon.API.BackgroundServices;

public class ChurnNotificationBackgroundService : BackgroundService
{
    private readonly ILogger<ChurnNotificationBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1); // Run every hour

    public ChurnNotificationBackgroundService(
        ILogger<ChurnNotificationBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Churn Notification Background Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SendAutomaticNotificationsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in churn notification background service.");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task SendAutomaticNotificationsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var churnNotificationService = scope.ServiceProvider.GetRequiredService<IChurnNotificationService>();
        var decisionEngineService = scope.ServiceProvider.GetRequiredService<IDecisionEngineService>();

        // Get high-risk customers
        var highRiskThreshold = 0.7; // Configurable
        var predictions = await scope.ServiceProvider.GetRequiredService<IPredictionDataRepository>()
            .GetPredictionsByChurnRiskAsync(highRiskThreshold, null);

        foreach (var prediction in predictions)
        {
            // Get recommended offer
            var decision = await decisionEngineService.EvaluateInterventionAsync(prediction.CustomerId.ToString());

            if (!string.IsNullOrEmpty(decision.SelectedOffer))
            {
                var channel = decision.DeliveryChannel ?? "email";
                var message = $"Recommended Offer: {decision.SelectedOffer}";

                await churnNotificationService.SendNotificationsToHighRiskCustomersAsync(
                    prediction.ChurnRiskScore.GetValueOrDefault(), channel, message);
            }
        }
    }
}