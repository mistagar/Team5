using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Team5Hackathon.Application.Services;
using Team5Hackathon.Domain.Entities;
using Team5Hackathon.Domain.RepositoriesContract;

namespace Team5Hackathon.Application.Services
{
    public class PredictionQueryService : IPredictionQueryService
    {
        private readonly IPredictionRepository _repository;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public PredictionQueryService(IPredictionRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
        }

        public async Task<CustomerRiskProfile?> GetByCustomerIdAsync(string customerId)
        {
            string cacheKey = $"CustomerRiskProfile_{customerId}";
            if (!_cache.TryGetValue(cacheKey, out CustomerRiskProfile? profile))
            {
                profile = await _repository.GetByCustomerIdAsync(customerId);
                if (profile != null)
                {
                    _cache.Set(cacheKey, profile, _cacheOptions);
                }
            }
            return profile;
        }

        public async Task<IEnumerable<CustomerRiskProfile>> GetByChurnRiskScoreRangeAsync(double minScore, double maxScore)
        {
            string cacheKey = $"ChurnRiskScoreRange_{minScore}_{maxScore}";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<CustomerRiskProfile>? profiles))
            {
                profiles = await _repository.GetByChurnRiskScoreRangeAsync(minScore, maxScore);
                _cache.Set(cacheKey, profiles, _cacheOptions);
            }
            return profiles;
        }

        public async Task<IEnumerable<CustomerRiskProfile>> GetByCustomerTypeAsync(string customerType)
        {
            string cacheKey = $"CustomerType_{customerType}";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<CustomerRiskProfile>? profiles))
            {
                profiles = await _repository.GetByCustomerTypeAsync(customerType);
                _cache.Set(cacheKey, profiles, _cacheOptions);
            }
            return profiles;
        }

        public async Task<IEnumerable<CustomerRiskProfile>> GetAllAsync()
        {
            string cacheKey = "AllCustomerRiskProfiles";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<CustomerRiskProfile>? profiles))
            {
                profiles = await _repository.GetAllAsync();
                _cache.Set(cacheKey, profiles, _cacheOptions);
            }
            return profiles;
        }
    }
}