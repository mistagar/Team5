using System.Threading.Tasks;

namespace Team5Hackathon.Application.Services;

public interface IChurnNotificationService
{
    Task<int> SendNotificationsToHighRiskCustomersAsync(double riskThreshold, string channel, string messageTemplate);
}