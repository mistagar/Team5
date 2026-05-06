using System.Collections.Generic;
using System.Threading.Tasks;
using Team5Hackathon.Domain.Entities;

namespace Team5Hackathon.Application.Services
{
    public interface IPredictionQueryService
    {
        Task<CustomerRiskProfile?> GetByCustomerIdAsync(string customerId);
        Task<IEnumerable<CustomerRiskProfile>> GetByChurnRiskScoreRangeAsync(double minScore, double maxScore);
        Task<IEnumerable<CustomerRiskProfile>> GetByCustomerTypeAsync(string customerType);
        Task<IEnumerable<CustomerRiskProfile>> GetAllAsync();
    }
}