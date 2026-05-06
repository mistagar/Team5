using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Team5Hackathon.Domain.Entities;
using Team5Hackathon.Domain.RepositoriesContract;
using Team5Hackathon.Infrastructure.Persistence;

namespace Team5Hackathon.Infrastructure.Repositories
{
    public class PredictionRepository : IPredictionRepository
    {
        private readonly AppDbContext _context;

        public PredictionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CustomerRiskProfile?> GetByCustomerIdAsync(string customerId)
        {
            return await _context.CustomerRiskProfiles
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<IEnumerable<CustomerRiskProfile>> GetByChurnRiskScoreRangeAsync(double minScore, double maxScore)
        {
            return await _context.CustomerRiskProfiles
                .Where(c => c.ChurnRiskScore >= minScore && c.ChurnRiskScore <= maxScore)
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomerRiskProfile>> GetByCustomerTypeAsync(string customerType)
        {
            return await _context.CustomerRiskProfiles
                .Where(c => c.CustomerType == customerType)
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomerRiskProfile>> GetAllAsync()
        {
            return await _context.CustomerRiskProfiles.ToListAsync();
        }
    }
}