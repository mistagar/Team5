using System;
using System.Collections.Generic;

namespace Team5Hackathon.Application.DTOs
{
    public class ChannelConfigurationDTO
    {
        public Guid Id { get; set; }
        public string ChannelType { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public int RetryCount { get; set; }
        public int CooldownMinutes { get; set; }
        public string? PreferredChurnTypes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateChannelDTO
    {
        public string ChannelType { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public int RetryCount { get; set; }
        public int CooldownMinutes { get; set; }
        public string? PreferredChurnTypes { get; set; }
    }

    public class UpdateChannelDTO
    {
        public string ChannelType { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public int RetryCount { get; set; }
        public int CooldownMinutes { get; set; }
        public string? PreferredChurnTypes { get; set; }
    }
}