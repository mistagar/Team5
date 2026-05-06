using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Team5Hackathon.Application.DTOs;
using Team5Hackathon.Domain.RepositoriesContract;

namespace Team5Hackathon.Application.Services;

public class ChurnNotificationService : IChurnNotificationService
{
    private readonly IPredictionDataRepository _predictionRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<ChurnNotificationService> _logger;

    public ChurnNotificationService(
        IPredictionDataRepository predictionRepository,
        INotificationService notificationService,
        ILogger<ChurnNotificationService> logger)
    {
        _predictionRepository = predictionRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<int> SendNotificationsToHighRiskCustomersAsync(double riskThreshold, string channel, string messageTemplate)
    {
        var highRiskPredictions = await _predictionRepository.GetPredictionsByChurnRiskAsync(riskThreshold, true);

        int sentCount = 0;
        foreach (var prediction in highRiskPredictions)
        {
            // Placeholder for recipient
            string recipient = prediction.CustomerId.ToString();

            string message = messageTemplate.Replace("{CustomerId}", prediction.CustomerId.ToString())
                                           .Replace("{ChurnRisk}", prediction.ChurnRiskScore.ToString());

            bool sent = channel.ToLower() switch
            {
                "email" => await _notificationService.SendEmailAsync(recipient, "Churn Prevention", message),
                "sms" => await _notificationService.SendSmsAsync(recipient, message),
                "whatsapp" => await _notificationService.SendWhatsAppAsync(recipient, message),
                _ => false
            };

            if (sent) sentCount++;
        }

        _logger.LogInformation("Sent {Count} notifications via {Channel}", sentCount, channel);
        return sentCount;
    }
}