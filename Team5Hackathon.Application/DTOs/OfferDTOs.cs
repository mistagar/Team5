using System;
using System.Collections.Generic;

namespace Team5Hackathon.Application.DTOs
{
    public class OfferConfigurationDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
        public DateTime ValidityStart { get; set; }
        public DateTime ValidityEnd { get; set; }
        public bool IsActive { get; set; }
        public string? OfferType { get; set; }
        public decimal Value { get; set; }
        public string? Terms { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<OfferEligibilityRuleDTO> EligibilityRules { get; set; } = new();
    }

    public class CreateOfferDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
        public DateTime ValidityStart { get; set; }
        public DateTime ValidityEnd { get; set; }
        public string? OfferType { get; set; }
        public decimal Value { get; set; }
        public string? Terms { get; set; }
    }

    public class UpdateOfferDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
        public DateTime ValidityStart { get; set; }
        public DateTime ValidityEnd { get; set; }
        public string? OfferType { get; set; }
        public decimal Value { get; set; }
        public string? Terms { get; set; }
    }

    public class OfferEligibilityRuleDTO
    {
        public Guid Id { get; set; }
        public Guid OfferConfigurationId { get; set; }
        public string? ChurnClassification { get; set; }
        public double? MinChurnRisk { get; set; }
        public double? MaxChurnRisk { get; set; }
        public double? MinEngagement { get; set; }
        public double? MaxEngagement { get; set; }
        public bool? HasComplaint { get; set; }
        public string? ComplaintText { get; set; }
        public double? MinNetworkQuality { get; set; }
        public double? MaxNetworkQuality { get; set; }
    }

    public class CreateEligibilityRuleDTO
    {
        public string? ChurnClassification { get; set; }
        public double? MinChurnRisk { get; set; }
        public double? MaxChurnRisk { get; set; }
        public double? MinEngagement { get; set; }
        public double? MaxEngagement { get; set; }
        public bool? HasComplaint { get; set; }
        public string? ComplaintText { get; set; }
        public double? MinNetworkQuality { get; set; }
        public double? MaxNetworkQuality { get; set; }
    }

    public class UpdateEligibilityRuleDTO
    {
        public string? ChurnClassification { get; set; }
        public double? MinChurnRisk { get; set; }
        public double? MaxChurnRisk { get; set; }
        public double? MinEngagement { get; set; }
        public double? MaxEngagement { get; set; }
        public bool? HasComplaint { get; set; }
        public string? ComplaintText { get; set; }
        public double? MinNetworkQuality { get; set; }
        public double? MaxNetworkQuality { get; set; }
    }
}