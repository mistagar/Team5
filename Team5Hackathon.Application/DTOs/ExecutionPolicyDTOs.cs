using System;
using System.Collections.Generic;

namespace Team5Hackathon.Application.DTOs
{
    public class ExecutionPolicyDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public double RiskThreshold { get; set; }
        public string? ApplicableSegments { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateExecutionPolicyDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public double RiskThreshold { get; set; }
        public string? ApplicableSegments { get; set; }
    }

    public class UpdateExecutionPolicyDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public double RiskThreshold { get; set; }
        public string? ApplicableSegments { get; set; }
    }
}