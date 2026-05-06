using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team5Hackathon.Domain.Entities;

namespace Team5Hackathon.Domain.RepositoriesContract
{
    public interface IOfferRepository
    {
        Task<OfferConfiguration?> GetOfferByIdAsync(Guid offerId);
        Task<IEnumerable<OfferConfiguration>> GetAllOffersAsync();
        Task<IEnumerable<OfferConfiguration>> GetActiveOffersAsync();
        Task<OfferConfiguration> CreateOfferAsync(OfferConfiguration offer);
        Task<bool> UpdateOfferAsync(OfferConfiguration offer);
        Task<bool> DeactivateOfferAsync(Guid offerId);
        Task<OfferEligibilityRule> AddEligibilityRuleAsync(OfferEligibilityRule rule);
        Task<bool> UpdateEligibilityRuleAsync(OfferEligibilityRule rule);
        Task<bool> DeleteEligibilityRuleAsync(Guid ruleId);
        Task<IEnumerable<OfferEligibilityRule>> GetEligibilityRulesByOfferIdAsync(Guid offerId);
        Task<OfferEligibilityRule?> GetEligibilityRuleByIdAsync(Guid ruleId);
    }
}