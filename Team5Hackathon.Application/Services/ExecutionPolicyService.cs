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
    public class ExecutionPolicyService : IExecutionPolicyService
    {
        private readonly IExecutionPolicyRepository _policyRepository;

        public ExecutionPolicyService(IExecutionPolicyRepository policyRepository)
        {
            _policyRepository = policyRepository;
        }

        public async Task<ExecutionPolicyDTO?> GetPolicyByIdAsync(Guid policyId)
        {
            var policy = await _policyRepository.GetPolicyByIdAsync(policyId);
            return policy == null ? null : MapToDTO(policy);
        }

        public async Task<IEnumerable<ExecutionPolicyDTO>> GetAllPoliciesAsync()
        {
            var policies = await _policyRepository.GetAllPoliciesAsync();
            return policies.Select(MapToDTO);
        }

        public async Task<IEnumerable<ExecutionPolicyDTO>> GetActivePoliciesAsync()
        {
            var policies = await _policyRepository.GetActivePoliciesAsync();
            return policies.Select(MapToDTO);
        }

        public async Task<ExecutionPolicyDTO> CreatePolicyAsync(CreateExecutionPolicyDTO dto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Policy name is required.");
            if (dto.Mode != "Automatic" && dto.Mode != "Manual")
                throw new ArgumentException("Mode must be Automatic or Manual.");
            if (dto.RiskThreshold < 0 || dto.RiskThreshold > 1)
                throw new ArgumentException("Risk threshold must be between 0 and 1.");

            var policy = new ExecutionPolicy
            {
                Name = dto.Name,
                Mode = dto.Mode,
                RiskThreshold = dto.RiskThreshold,
                ApplicableSegments = dto.ApplicableSegments,
                IsActive = true
            };

            var created = await _policyRepository.CreatePolicyAsync(policy);
            return MapToDTO(created);
        }

        public async Task<bool> UpdatePolicyAsync(Guid policyId, UpdateExecutionPolicyDTO dto)
        {
            var policy = await _policyRepository.GetPolicyByIdAsync(policyId);
            if (policy == null) return false;

            // Validation
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Policy name is required.");
            if (dto.Mode != "Automatic" && dto.Mode != "Manual")
                throw new ArgumentException("Mode must be Automatic or Manual.");
            if (dto.RiskThreshold < 0 || dto.RiskThreshold > 1)
                throw new ArgumentException("Risk threshold must be between 0 and 1.");

            policy.Name = dto.Name;
            policy.Mode = dto.Mode;
            policy.RiskThreshold = dto.RiskThreshold;
            policy.ApplicableSegments = dto.ApplicableSegments;

            return await _policyRepository.UpdatePolicyAsync(policy);
        }

        public async Task<bool> DeletePolicyAsync(Guid policyId)
        {
            return await _policyRepository.DeletePolicyAsync(policyId);
        }

        public async Task<string> EvaluateExecutionModeAsync(double riskScore, string customerType)
        {
            var activePolicies = await _policyRepository.GetActivePoliciesAsync();
            var applicablePolicy = activePolicies.FirstOrDefault(p =>
                (string.IsNullOrEmpty(p.ApplicableSegments) ||
                 p.ApplicableSegments.Split(',').Contains(customerType.Trim())) &&
                riskScore >= p.RiskThreshold);

            if (applicablePolicy != null)
            {
                return applicablePolicy.Mode;
            }

            // Default to Automatic for low risk
            return "Automatic";
        }

        private ExecutionPolicyDTO MapToDTO(ExecutionPolicy policy)
        {
            return new ExecutionPolicyDTO
            {
                Id = policy.Id,
                Name = policy.Name,
                Mode = policy.Mode,
                RiskThreshold = policy.RiskThreshold,
                ApplicableSegments = policy.ApplicableSegments,
                IsActive = policy.IsActive,
                CreatedAt = policy.CreatedAt,
                UpdatedAt = policy.UpdatedAt
            };
        }
    }
}