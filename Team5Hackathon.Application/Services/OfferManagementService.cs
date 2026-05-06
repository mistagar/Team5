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
    public class OfferManagementService : IOfferManagementService
    {
        private readonly IOfferRepository _offerRepository;

        public OfferManagementService(IOfferRepository offerRepository)
        {
            _offerRepository = offerRepository;
        }

        public async Task<OfferConfigurationDTO?> GetOfferByIdAsync(Guid offerId)
        {
            var offer = await _offerRepository.GetOfferByIdAsync(offerId);
            return offer == null ? null : MapToDTO(offer);
        }

        public async Task<IEnumerable<OfferConfigurationDTO>> GetAllOffersAsync()
        {
            var offers = await _offerRepository.GetAllOffersAsync();
            return offers.Select(MapToDTO);
        }

        public async Task<IEnumerable<OfferConfigurationDTO>> GetActiveOffersAsync()
        {
            var offers = await _offerRepository.GetActiveOffersAsync();
            return offers.Select(MapToDTO);
        }

        public async Task<OfferConfigurationDTO> CreateOfferAsync(CreateOfferDTO dto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Offer name is required.");
            if (dto.ValidityStart >= dto.ValidityEnd)
                throw new ArgumentException("Validity start must be before end.");
            if (dto.Value <= 0)
                throw new ArgumentException("Offer value must be positive.");

            var offer = new OfferConfiguration
            {
                Name = dto.Name,
                Description = dto.Description,
                Priority = dto.Priority,
                ValidityStart = dto.ValidityStart,
                ValidityEnd = dto.ValidityEnd,
                IsActive = true,
                OfferType = dto.OfferType,
                Value = dto.Value,
                Terms = dto.Terms
            };

            var created = await _offerRepository.CreateOfferAsync(offer);
            return MapToDTO(created);
        }

        public async Task<bool> UpdateOfferAsync(Guid offerId, UpdateOfferDTO dto)
        {
            var offer = await _offerRepository.GetOfferByIdAsync(offerId);
            if (offer == null) return false;

            // Validation
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Offer name is required.");
            if (dto.ValidityStart >= dto.ValidityEnd)
                throw new ArgumentException("Validity start must be before end.");
            if (dto.Value <= 0)
                throw new ArgumentException("Offer value must be positive.");

            offer.Name = dto.Name;
            offer.Description = dto.Description;
            offer.Priority = dto.Priority;
            offer.ValidityStart = dto.ValidityStart;
            offer.ValidityEnd = dto.ValidityEnd;
            offer.OfferType = dto.OfferType;
            offer.Value = dto.Value;
            offer.Terms = dto.Terms;

            return await _offerRepository.UpdateOfferAsync(offer);
        }

        public async Task<bool> DeactivateOfferAsync(Guid offerId)
        {
            return await _offerRepository.DeactivateOfferAsync(offerId);
        }

        public async Task<OfferEligibilityRuleDTO> AddEligibilityRuleAsync(Guid offerId, CreateEligibilityRuleDTO dto)
        {
            var offer = await _offerRepository.GetOfferByIdAsync(offerId);
            if (offer == null)
                throw new ArgumentException("Offer not found.");

            var rule = new OfferEligibilityRule
            {
                OfferConfigurationId = offerId,
                ChurnClassification = dto.ChurnClassification,
                MinChurnRisk = dto.MinChurnRisk,
                MaxChurnRisk = dto.MaxChurnRisk,
                MinEngagement = dto.MinEngagement,
                MaxEngagement = dto.MaxEngagement,
                HasComplaint = dto.HasComplaint,
                ComplaintText = dto.ComplaintText,
                MinNetworkQuality = dto.MinNetworkQuality,
                MaxNetworkQuality = dto.MaxNetworkQuality
            };

            var added = await _offerRepository.AddEligibilityRuleAsync(rule);
            return MapRuleToDTO(added);
        }

        public async Task<bool> UpdateEligibilityRuleAsync(Guid ruleId, UpdateEligibilityRuleDTO dto)
        {
            var rule = await _offerRepository.GetEligibilityRuleByIdAsync(ruleId);
            if (rule == null) return false;

            rule.ChurnClassification = dto.ChurnClassification;
            rule.MinChurnRisk = dto.MinChurnRisk;
            rule.MaxChurnRisk = dto.MaxChurnRisk;
            rule.MinEngagement = dto.MinEngagement;
            rule.MaxEngagement = dto.MaxEngagement;
            rule.HasComplaint = dto.HasComplaint;
            rule.ComplaintText = dto.ComplaintText;
            rule.MinNetworkQuality = dto.MinNetworkQuality;
            rule.MaxNetworkQuality = dto.MaxNetworkQuality;

            return await _offerRepository.UpdateEligibilityRuleAsync(rule);
        }

        public async Task<bool> DeleteEligibilityRuleAsync(Guid ruleId)
        {
            return await _offerRepository.DeleteEligibilityRuleAsync(ruleId);
        }

        public async Task<IEnumerable<OfferEligibilityRuleDTO>> GetEligibilityRulesByOfferIdAsync(Guid offerId)
        {
            var rules = await _offerRepository.GetEligibilityRulesByOfferIdAsync(offerId);
            return rules.Select(MapRuleToDTO);
        }

        private OfferConfigurationDTO MapToDTO(OfferConfiguration offer)
        {
            return new OfferConfigurationDTO
            {
                Id = offer.Id,
                Name = offer.Name,
                Description = offer.Description,
                Priority = offer.Priority,
                ValidityStart = offer.ValidityStart,
                ValidityEnd = offer.ValidityEnd,
                IsActive = offer.IsActive,
                OfferType = offer.OfferType,
                Value = offer.Value,
                Terms = offer.Terms,
                CreatedAt = offer.CreatedAt,
                UpdatedAt = offer.UpdatedAt,
                EligibilityRules = offer.EligibilityRules.Select(MapRuleToDTO).ToList()
            };
        }

        private OfferEligibilityRuleDTO MapRuleToDTO(OfferEligibilityRule rule)
        {
            return new OfferEligibilityRuleDTO
            {
                Id = rule.Id,
                OfferConfigurationId = rule.OfferConfigurationId,
                ChurnClassification = rule.ChurnClassification,
                MinChurnRisk = rule.MinChurnRisk,
                MaxChurnRisk = rule.MaxChurnRisk,
                MinEngagement = rule.MinEngagement,
                MaxEngagement = rule.MaxEngagement,
                HasComplaint = rule.HasComplaint,
                ComplaintText = rule.ComplaintText,
                MinNetworkQuality = rule.MinNetworkQuality,
                MaxNetworkQuality = rule.MaxNetworkQuality
            };
        }
    }
}