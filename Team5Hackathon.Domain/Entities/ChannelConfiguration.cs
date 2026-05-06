using System.ComponentModel.DataAnnotations;

namespace Team5Hackathon.Domain.Entities
{
    public class ChannelConfiguration
    {
        [Key]
        public Guid Id { get; set; }
        public string ChannelType { get; set; } = string.Empty; // SMS, USSD, WhatsApp, Push
        public bool IsEnabled { get; set; }
        public int RetryCount { get; set; }
        public int CooldownMinutes { get; set; }
        public string? PreferredChurnTypes { get; set; } // Comma-separated list of churn classifications
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}