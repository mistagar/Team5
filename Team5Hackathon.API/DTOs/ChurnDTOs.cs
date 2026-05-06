namespace Team5Hackathon.API.DTOs;

public class ChurnCustomerDTO
{
    public int CustomerId { get; set; }
    public double ChurnRiskScore { get; set; }
    public List<OfferSummaryDTO> RecommendedOffers { get; set; } = new();
    public string? RecommendedChannel { get; set; }
}

public class OfferSummaryDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class SendToCustomerRequest
{
    public int CustomerId { get; set; }
    public string OfferName { get; set; } = string.Empty;
    public string OfferDescription { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty; // email, sms, whatsapp
}