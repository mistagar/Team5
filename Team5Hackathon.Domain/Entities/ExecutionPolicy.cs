using System.ComponentModel.DataAnnotations;

namespace Team5Hackathon.Domain.Entities
{
    public class ExecutionPolicy
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Mode { get; set; } = "Automatic"; // Automatic or Manual
        public double RiskThreshold { get; set; } // Above this, manual approval
        public string? ApplicableSegments { get; set; } // Comma-separated
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}