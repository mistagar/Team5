using Microsoft.EntityFrameworkCore;
using Team5Hackathon.Domain.Entities;
using Team5Hackathon.Domain.RepositoriesContract;
using Team5Hackathon.Infrastructure.Persistence;

namespace Team5Hackathon.Infrastructure.Repositories
{
    public class PredictionDataRepository : IPredictionDataRepository
    {
        private readonly AppDbContext _context;

        public PredictionDataRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Prediction> Predictions, int TotalCount)> GetAllPredictionsAsync(int page, int pageSize, string? customerType = null, double? minChurnRisk = null, double? maxChurnRisk = null, bool? churn = null)
        {
            var query = _context.Predictions.AsQueryable();

            if (!string.IsNullOrEmpty(customerType))
            {
                query = query.Where(p => p.CustomerType == customerType);
            }

            if (minChurnRisk.HasValue)
            {
                query = query.Where(p => p.ChurnRiskScore >= minChurnRisk.Value);
            }

            if (maxChurnRisk.HasValue)
            {
                query = query.Where(p => p.ChurnRiskScore <= maxChurnRisk.Value);
            }

            if (churn.HasValue)
            {
                query = query.Where(p => p.Churn == churn.Value);
            }

            var totalCount = await query.CountAsync();

            var predictions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (predictions, totalCount);
        }

        public async Task<Prediction?> GetPredictionByIdAsync(int id)
        {
            return await _context.Predictions
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<(IEnumerable<Prediction> Predictions, int TotalCount)> GetPredictionsByCustomerIdAsync(int customerId, int page = 1, int pageSize = 10)
        {
            var query = _context.Predictions.Where(p => p.CustomerId == customerId);

            var totalCount = await query.CountAsync();

            var predictions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (predictions, totalCount);
        }
    }
}