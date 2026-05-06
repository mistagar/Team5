using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Team5Hackathon.Application.DTOs;
using Team5Hackathon.Application.Services;
using Team5Hackathon.Domain.Entities;
using Team5Hackathon.Domain.RepositoriesContract;

namespace Team5Hackathon.Application.Services
{
    public interface IExecutionPolicyService
    {
        Task<ExecutionPolicyDTO?> GetPolicyByIdAsync(Guid policyId);
        Task<IEnumerable<ExecutionPolicyDTO>> GetAllPoliciesAsync();
        Task<IEnumerable<ExecutionPolicyDTO>> GetActivePoliciesAsync();
        Task<ExecutionPolicyDTO> CreatePolicyAsync(CreateExecutionPolicyDTO dto);
        Task<bool> UpdatePolicyAsync(Guid policyId, UpdateExecutionPolicyDTO dto);
        Task<bool> DeletePolicyAsync(Guid policyId);
        Task<string> EvaluateExecutionModeAsync(double riskScore, string customerType);
    }
}