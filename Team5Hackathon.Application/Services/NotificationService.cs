using System.Net;
using System.Net.Mail;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Team5Hackathon.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<NotificationService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public NotificationService(
        IConfiguration configuration,
        ILogger<NotificationService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
    {
        _logger.LogInformation("SIMULATED EMAIL: To: {Email}, Subject: {Subject}, Body: {Body}", toEmail, subject, body);
        return true;
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        _logger.LogInformation("SIMULATED SMS: To: {Phone}, Message: {Message}", phoneNumber, message);
        return true;
    }

    public async Task<bool> SendWhatsAppAsync(string phoneNumber, string message)
    {
        _logger.LogInformation("SIMULATED WhatsApp: To: {Phone}, Message: {Message}", phoneNumber, message);
        return true;
    }
}