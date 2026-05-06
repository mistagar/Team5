using System.ComponentModel.DataAnnotations;

namespace Team5Hackathon.Domain.Entities
{
    public class OfferConfiguration
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
        public DateTime ValidityStart { get; set; }
        public DateTime ValidityEnd { get; set; }
        public bool IsActive { get; set; }
        public string? OfferType { get; set; } // e.g., DataBundle, WinBack
        public decimal Value { get; set; }
        public string? Terms { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<OfferEligibilityRule> EligibilityRules { get; set; } = new List<OfferEligibilityRule>();
    }
}