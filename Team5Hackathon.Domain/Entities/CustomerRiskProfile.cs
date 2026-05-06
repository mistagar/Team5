using System.ComponentModel.DataAnnotations;

namespace Team5Hackathon.Domain.Entities
{
    public class CustomerRiskProfile
    {
        [Key]
        public string CustomerId { get; set; } = string.Empty;
        public double AvgDataUsage { get; set; }
        public double UsageChangePct { get; set; }
        public int RechargeFreq { get; set; }
        public double RechargeChangePct { get; set; }
        public double EngagementScore { get; set; }
        public double NetworkQuality { get; set; }
        public bool HasComplaint { get; set; }
        public string? ComplaintText { get; set; }
        public bool Churn { get; set; }
        public double ChurnRiskScore { get; set; }
        public string CustomerType { get; set; } = string.Empty;
    }
}