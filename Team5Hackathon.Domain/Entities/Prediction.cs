using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team5Hackathon.Domain.Entities
{
    //public class Prediction
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    [Column("customer_id")]
    //    public int CustomerId { get; set; }
    //    [Column("avg_data_usage")]
    //    public double AvgDataUsage { get; set; }
    //    [Column("usage_change_pct")]
    //    public double UsageChangePct { get; set; }
    //    [Column("recharge_freq")]
    //    public int RechargeFreq { get; set; }
    //    [Column("recharge_change_pct")]
    //    public double RechargeChangePct { get; set; }
    //    [Column("engagement_score")]
    //    public double EngagementScore { get; set; }
    //    [Column("network_quality")]
    //    public double NetworkQuality { get; set; }
    //    [Column("has_complaint")]
    //    public int HasComplaint { get; set; } // 0 or 1
    //    [Column("complaint_text")]
    //    public string? ComplaintText { get; set; }
    //    [Column("churn")]
    //    public int Churn { get; set; } // 0 or 1
    //    [Column("churn_risk_score")]
    //    public double ChurnRiskScore { get; set; }
    //    [Column("customer_type")]
    //    public string CustomerType { get; set; } = string.Empty;
    //    [Column("timestamp")]
    //    public string Timestamp { get; set; } = string.Empty;
    //}

    [Table("predictions", Schema = "dbo")]
    public class Prediction
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("customer_id")]
        public int? CustomerId { get; set; }

        [Column("avg_data_usage")]
        public double? AvgDataUsage { get; set; }

        [Column("usage_change_pct")]
        public double? UsageChangePct { get; set; }

        [Column("recharge_freq")]
        public int? RechargeFreq { get; set; }

        [Column("recharge_change_pct")]
        public double? RechargeChangePct { get; set; }

        [Column("engagement_score")]
        public double? EngagementScore { get; set; }

        [Column("network_quality")]
        public double? NetworkQuality { get; set; }

        [Column("has_complaint")]
        public bool? HasComplaint { get; set; }

        [Column("complaint_text")]
        public string? ComplaintText { get; set; }

        [Column("churn")]
        public bool? Churn { get; set; }

        [Column("churn_risk_score")]
        public double? ChurnRiskScore { get; set; }

        [Column("customer_type")]
        public string? CustomerType { get; set; }

        [Column("timestamp")]
        public DateTime? Timestamp { get; set; }
    }
}