using System;
using Team5Hackathon.Application.DTOs.UserDTO;

namespace Team5Hackathon.Application.DTOs
{
    public class CallDTO
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Transcript { get; set; }
        public string? Summary { get; set; }
        public string? ActionItems { get; set; }
        public string? PrimaryIntent { get; set; }
        public double? SatisfactionRating { get; set; }
        public bool IsResolved { get; set; }
        public string? Status { get; set; }
    }

    public class StartCallDTO
    {
        public Guid ClientId { get; set; }
    }

    public class EndCallDTO
    {
        public Guid CallId { get; set; }
        public string? Transcript { get; set; }
        public double? SatisfactionRating { get; set; }
        public bool IsResolved { get; set; }
    }

    public class TranscriptSegmentDTO
    {
        public Guid CallId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Text { get; set; }
    }

    public class FollowUpMessageDTO
    {
        public Guid Id { get; set; }
        public Guid CallId { get; set; }
        public string? Type { get; set; }
        public string? Content { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? SentAt { get; set; }
        public string? DeliveryStatus { get; set; }
    }

    public class GenerateFollowUpDTO
    {
        public Guid CallId { get; set; }
        public string Type { get; set; } // SMS, WhatsApp, Email
    }

    public class DashboardMetricsDTO
    {
        public int TotalClientsAttended { get; set; }
        public int IssuesResolved { get; set; }
        public int IssuesPending { get; set; }
        public double? AverageSatisfactionRating { get; set; }
    }

    public class ClientDashboardDTO
    {
        public IEnumerable<CallDTO>? CallHistory { get; set; }
        public IEnumerable<UserRequestDTO>? RequestHistory { get; set; }
    }
}