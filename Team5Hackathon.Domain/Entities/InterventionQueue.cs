using System.ComponentModel.DataAnnotations;

namespace Team5Hackathon.Domain.Entities
{
    public class InterventionQueue
    {
        [Key]
        public Guid Id { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string RecommendedOffer { get; set; } = string.Empty;
        public string RecommendedChannel { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Executed
        public string? ApprovedOffer { get; set; }
        public string? ApprovedChannel { get; set; }
        public Guid? ApprovedBy { get; set; }
        public string? ApprovalNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? ExecutedAt { get; set; }
    }
}