using System;
using System.Collections.Generic;

namespace Team5Hackathon.Application.DTOs
{
    public class InterventionQueueDTO
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string RecommendedOffer { get; set; } = string.Empty;
        public string RecommendedChannel { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ApprovedOffer { get; set; }
        public string? ApprovedChannel { get; set; }
        public Guid? ApprovedBy { get; set; }
        public string? ApprovalNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? ExecutedAt { get; set; }
    }

    public class ApproveInterventionDTO
    {
        public string? ModifiedOffer { get; set; }
        public string? ModifiedChannel { get; set; }
        public string? Notes { get; set; }
    }

    public class RejectInterventionDTO
    {
        public string Notes { get; set; } = string.Empty;
    }
}