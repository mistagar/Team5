using System;
using System.ComponentModel.DataAnnotations;

namespace Team5Hackathon.Domain.Entities
{
    public class UserRequest
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        
        public Guid? CallId { get; set; }

        public string Type { get; set; } = string.Empty; // "call", "voice_note", "enquiry"
        public string? Content { get; set; }  // transcript or complaint text
        public DateTime CreatedAt { get; set; }
        public string? Status { get; set; }   // "pending", "processed", "resolved"
        public string? Summary { get; set; }
        public string? Response { get; set; }
        
        public string? Category { get; set; }
        
        public string? Sentiment { get; set; }
    }
}