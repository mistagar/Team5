using System;
using System.ComponentModel.DataAnnotations;

namespace Team5Hackathon.Domain.Entities
{
    public class FollowUpMessage
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CallId { get; set; }
        public string Type { get; set; } // "SMS", "WhatsApp", "Email"
        public string Content { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? SentAt { get; set; }
        public string? DeliveryStatus { get; set; } // "pending", "sent", "failed"
    }
}