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
    public class OfferRepository : IOfferRepository
    {
        private readonly AppDbContext _context;

        public OfferRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OfferConfiguration?> GetOfferByIdAsync(Guid offerId)
        {
            return await _context.OfferConfigurations
                .Include(o => o.EligibilityRules)
                .FirstOrDefaultAsync(o => o.Id == offerId);
        }

        public async Task<IEnumerable<OfferConfiguration>> GetAllOffersAsync()
        {
            return await _context.OfferConfigurations
                .Include(o => o.EligibilityRules)
                .ToListAsync();
        }

        public async Task<IEnumerable<OfferConfiguration>> GetActiveOffersAsync()
        {
            return await _context.OfferConfigurations
                .Include(o => o.EligibilityRules)
                .Where(o => o.IsActive && o.ValidityStart <= DateTime.Now && o.ValidityEnd >= DateTime.Now)
                .ToListAsync();
        }

        public async Task<OfferConfiguration> CreateOfferAsync(OfferConfiguration offer)
        {
            offer.CreatedAt = DateTime.UtcNow;
            _context.OfferConfigurations.Add(offer);
            await _context.SaveChangesAsync();
            return offer;
        }

        public async Task<bool> UpdateOfferAsync(OfferConfiguration offer)
        {
            offer.UpdatedAt = DateTime.UtcNow;
            _context.OfferConfigurations.Update(offer);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeactivateOfferAsync(Guid offerId)
        {
            var offer = await _context.OfferConfigurations.FindAsync(offerId);
            if (offer == null) return false;
            offer.IsActive = false;
            offer.UpdatedAt = DateTime.UtcNow;
            _context.OfferConfigurations.Update(offer);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<OfferEligibilityRule> AddEligibilityRuleAsync(OfferEligibilityRule rule)
        {
            _context.OfferEligibilityRules.Add(rule);
            await _context.SaveChangesAsync();
            return rule;
        }

        public async Task<bool> UpdateEligibilityRuleAsync(OfferEligibilityRule rule)
        {
            _context.OfferEligibilityRules.Update(rule);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteEligibilityRuleAsync(Guid ruleId)
        {
            var rule = await _context.OfferEligibilityRules.FindAsync(ruleId);
            if (rule == null) return false;
            _context.OfferEligibilityRules.Remove(rule);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<OfferEligibilityRule>> GetEligibilityRulesByOfferIdAsync(Guid offerId)
        {
            return await _context.OfferEligibilityRules
                .Where(r => r.OfferConfigurationId == offerId)
                .ToListAsync();
        }

        public async Task<OfferEligibilityRule?> GetEligibilityRuleByIdAsync(Guid ruleId)
        {
            return await _context.OfferEligibilityRules.FindAsync(ruleId);
        }
    }
}