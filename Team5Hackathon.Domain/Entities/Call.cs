using System;
using System.ComponentModel.DataAnnotations;

namespace Team5Hackathon.Domain.Entities
{
    public class Call
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; } // User Id
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Transcript { get; set; }
        public string? Summary { get; set; }
        public string? ActionItems { get; set; }
        public string? PrimaryIntent { get; set; } // e.g., "complaint", "enquiry"
        public string? Category { get; set; } 
        public string? Sentiment { get; set; } 
        public double? SatisfactionRating { get; set; }
        public bool IsResolved { get; set; }
        public string? Status { get; set; } // e.g., "active", "ended"
       
        public Guid? RequestId { get; set; }
    }
}