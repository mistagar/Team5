namespace Team5Hackathon.API.DTOs;

public class SendChurnNotificationsRequest
{
    public double RiskThreshold { get; set; }
    public string Channel { get; set; } = string.Empty; // email, sms, whatsapp
    public string MessageTemplate { get; set; } = string.Empty;
}