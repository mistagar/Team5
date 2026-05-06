using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team5Hackathon.Domain.Entities;

namespace Team5Hackathon.Domain.RepositoriesContract
{
    public interface IExecutionPolicyRepository
    {
        Task<ExecutionPolicy?> GetPolicyByIdAsync(Guid policyId);
        Task<IEnumerable<ExecutionPolicy>> GetAllPoliciesAsync();
        Task<IEnumerable<ExecutionPolicy>> GetActivePoliciesAsync();
        Task<ExecutionPolicy> CreatePolicyAsync(ExecutionPolicy policy);
        Task<bool> UpdatePolicyAsync(ExecutionPolicy policy);
        Task<bool> DeletePolicyAsync(Guid policyId);
    }
}