using System;
using System.ComponentModel.DataAnnotations;

namespace Team5Hackathon.Domain.Entities
{
    public class TranscriptSegment
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CallId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Text { get; set; }
        public string? Intent { get; set; }
        public string? Entities { get; set; } // JSON or string
    }
}