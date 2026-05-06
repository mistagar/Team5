using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team5Hackathon.Application.DTOs;

namespace Team5Hackathon.Application.Services
{
    public interface IOfferManagementService
    {
        Task<OfferConfigurationDTO?> GetOfferByIdAsync(Guid offerId);
        Task<IEnumerable<OfferConfigurationDTO>> GetAllOffersAsync();
        Task<IEnumerable<OfferConfigurationDTO>> GetActiveOffersAsync();
        Task<OfferConfigurationDTO> CreateOfferAsync(CreateOfferDTO dto);
        Task<bool> UpdateOfferAsync(Guid offerId, UpdateOfferDTO dto);
        Task<bool> DeactivateOfferAsync(Guid offerId);
        Task<OfferEligibilityRuleDTO> AddEligibilityRuleAsync(Guid offerId, CreateEligibilityRuleDTO dto);
        Task<bool> UpdateEligibilityRuleAsync(Guid ruleId, UpdateEligibilityRuleDTO dto);
        Task<bool> DeleteEligibilityRuleAsync(Guid ruleId);
        Task<IEnumerable<OfferEligibilityRuleDTO>> GetEligibilityRulesByOfferIdAsync(Guid offerId);
    }
}