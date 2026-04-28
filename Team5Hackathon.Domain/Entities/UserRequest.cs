using System;
using System.ComponentModel.DataAnnotations;

namespace Team5Hackathon.Domain.Entities
{
    public class UserRequest
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; } // "call", "voice_note", "enquiry"
        public string? Content { get; set; } // transcript or text
        public DateTime CreatedAt { get; set; }
        public string? Status { get; set; } // "pending", "processed", "resolved"
        public string? Summary { get; set; }
        public string? Response { get; set; }
    }
}