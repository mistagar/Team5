using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Team5Hackathon.Domain.Entities;
using Team5Hackathon.Domain.RepositoriesContract;
using Team5Hackathon.Infrastructure.Persistence;

namespace Team5Hackathon.Infrastructure.Repositories
{
    public class ExecutionPolicyRepository : IExecutionPolicyRepository
    {
        private readonly AppDbContext _context;

        public ExecutionPolicyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ExecutionPolicy?> GetPolicyByIdAsync(Guid policyId)
        {
            return await _context.ExecutionPolicies.FindAsync(policyId);
        }

        public async Task<IEnumerable<ExecutionPolicy>> GetAllPoliciesAsync()
        {
            return await _context.ExecutionPolicies.ToListAsync();
        }

        public async Task<IEnumerable<ExecutionPolicy>> GetActivePoliciesAsync()
        {
            return await _context.ExecutionPolicies.Where(p => p.IsActive).ToListAsync();
        }

        public async Task<ExecutionPolicy> CreatePolicyAsync(ExecutionPolicy policy)
        {
            policy.CreatedAt = DateTime.UtcNow;
            _context.ExecutionPolicies.Add(policy);
            await _context.SaveChangesAsync();
            return policy;
        }

        public async Task<bool> UpdatePolicyAsync(ExecutionPolicy policy)
        {
            policy.UpdatedAt = DateTime.UtcNow;
            _context.ExecutionPolicies.Update(policy);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePolicyAsync(Guid policyId)
        {
            var policy = await _context.ExecutionPolicies.FindAsync(policyId);
            if (policy == null) return false;
            _context.ExecutionPolicies.Remove(policy);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}