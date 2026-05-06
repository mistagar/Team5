using System.Threading.Tasks;

namespace Team5Hackathon.Application.Services;

public interface INotificationService
{
    Task<bool> SendEmailAsync(string toEmail, string subject, string body);
    Task<bool> SendSmsAsync(string phoneNumber, string message);
    Task<bool> SendWhatsAppAsync(string phoneNumber, string message);
}