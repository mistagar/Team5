using Microsoft.AspNetCore.Mvc;
using Team5Hackathon.API.DTOs;
using Team5Hackathon.Application.Services;
using Team5Hackathon.Application.DTOs;

namespace Team5Hackathon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChurnController : ControllerBase
{
    private readonly IChurnNotificationService _churnNotificationService;
    private readonly IPredictionDataService _predictionDataService;
    private readonly IDecisionEngineService _decisionEngineService;
    private readonly INotificationService _notificationService;

    public ChurnController(
        IChurnNotificationService churnNotificationService,
        IPredictionDataService predictionDataService,
        IDecisionEngineService decisionEngineService,
        INotificationService notificationService)
    {
        _churnNotificationService = churnNotificationService;
        _predictionDataService = predictionDataService;
        _decisionEngineService = decisionEngineService;
        _notificationService = notificationService;
    }

    [HttpPost("send-notifications")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<IActionResult> SendNotifications([FromBody] SendChurnNotificationsRequest request)
    {
        var sentCount = await _churnNotificationService.SendNotificationsToHighRiskCustomersAsync(
            request.RiskThreshold, request.Channel, request.MessageTemplate);

        return Ok(ApiResponse<int>.SuccessResponse(sentCount, $"Sent {sentCount} notifications."));
    }

    [HttpGet("high-risk-customers")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ChurnCustomerDTO>>), 200)]
    public async Task<IActionResult> GetHighRiskCustomers([FromQuery] double riskThreshold = 0.5)
    {
        var (predictions, _) = await _predictionDataService.GetAllPredictionsAsync(1, int.MaxValue, null, riskThreshold, null, null);

        var customers = new List<ChurnCustomerDTO>();
        foreach (var prediction in predictions)
        {
            var decision = await _decisionEngineService.EvaluateInterventionAsync(prediction.CustomerId.ToString());

            customers.Add(new ChurnCustomerDTO
            {
                CustomerId = prediction.CustomerId ?? 0,
                ChurnRiskScore = prediction.ChurnRiskScore.GetValueOrDefault(),
                RecommendedOffers = decision.SelectedOffer != null ? new List<OfferSummaryDTO> { new OfferSummaryDTO { Name = decision.SelectedOffer, Description = "Selected offer" } } : new List<OfferSummaryDTO>(),
                RecommendedChannel = decision.DeliveryChannel
            });
        }

        return Ok(ApiResponse<IEnumerable<ChurnCustomerDTO>>.SuccessResponse(customers));
    }

    [HttpPost("send-to-customer")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> SendToCustomer([FromBody] SendToCustomerRequest request)
    {
        // Assume CustomerId is email for email, phone for sms/whatsapp
        var recipient = request.CustomerId.ToString(); // Placeholder
        var message = $"Offer: {request.OfferName} - {request.OfferDescription}";

        bool sent = request.Channel.ToLower() switch
        {
            "email" => await _notificationService.SendEmailAsync(recipient, "Churn Prevention Offer", message),
            "sms" => await _notificationService.SendSmsAsync(recipient, message),
            "whatsapp" => await _notificationService.SendWhatsAppAsync(recipient, message),
            _ => false
        };

        return sent ? Ok(ApiResponse<string>.SuccessResponse("Notification sent.")) :
                      BadRequest(ApiResponse<string>.FailResponse("Failed to send."));
    }
}