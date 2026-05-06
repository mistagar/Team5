using System.ComponentModel.DataAnnotations;

namespace Team5Hackathon.Domain.Entities
{
    public class OfferEligibilityRule
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OfferConfigurationId { get; set; }
        public string? ChurnClassification { get; set; } // e.g., Vocal Churn, Healthy
        public double? MinChurnRisk { get; set; }
        public double? MaxChurnRisk { get; set; }
        public double? MinEngagement { get; set; }
        public double? MaxEngagement { get; set; }
        public bool? HasComplaint { get; set; }
        public string? ComplaintText { get; set; }
        public double? MinNetworkQuality { get; set; }
        public double? MaxNetworkQuality { get; set; }

        // Navigation
        public OfferConfiguration OfferConfiguration { get; set; } = null!;
    }
}