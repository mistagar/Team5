namespace Team5Hackathon.Application.DTOs
{
    public class DecisionEngineResponse
    {
        public bool ShouldIntervene { get; set; }
        public string? SelectedOffer { get; set; }
        public string? DeliveryChannel { get; set; }
        public string? Timing { get; set; }
        public double InterventionScore { get; set; }
        public string? Priority { get; set; }
        public string? ExecutionMode { get; set; }
        public List<string> Explanation { get; set; } = new();
    }
}