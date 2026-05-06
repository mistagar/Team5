namespace Team5Hackathon.Application.DTOs
{
    public class PredictionDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public double AvgDataUsage { get; set; }
        public double UsageChangePct { get; set; }
        public int RechargeFreq { get; set; }
        public double RechargeChangePct { get; set; }
        public double EngagementScore { get; set; }
        public double NetworkQuality { get; set; }
        public int HasComplaint { get; set; }
        public string? ComplaintText { get; set; }
        public int Churn { get; set; }
        public double ChurnRiskScore { get; set; }
        public string CustomerType { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
    }

    public class DashboardPredictionResponseDTO
    {
        public IEnumerable<PredictionDTO> Predictions { get; set; } = new List<PredictionDTO>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}