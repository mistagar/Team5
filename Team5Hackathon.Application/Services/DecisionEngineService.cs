using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Team5Hackathon.Application.DTOs;
using Team5Hackathon.Application.Services;
using Team5Hackathon.Domain.Entities;

namespace Team5Hackathon.Application.Services
{
    public class DecisionEngineService : IDecisionEngineService
    {
        private readonly IPredictionQueryService _predictionService;
        private readonly IConfiguration _configuration;
        private readonly IChannelManagementService _channelService;
        private readonly IExecutionPolicyService _policyService;
        private readonly ISupervisorApprovalService _approvalService;
        private readonly IOfferManagementService _offerService;

        public DecisionEngineService(IPredictionQueryService predictionService, IConfiguration configuration, IChannelManagementService channelService, IExecutionPolicyService policyService, ISupervisorApprovalService approvalService, IOfferManagementService offerService)
        {
            _predictionService = predictionService;
            _configuration = configuration;
            _channelService = channelService;
            _policyService = policyService;
            _approvalService = approvalService;
            _offerService = offerService;
        }

        public async Task<DecisionEngineResponse> EvaluateInterventionAsync(string customerId)
        {
            var response = new DecisionEngineResponse();

            // Fetch customer risk profile
            var profile = await _predictionService.GetByCustomerIdAsync(customerId);
            if (profile == null)
            {
                response.Explanation.Add("Customer risk profile not found.");
                return response;
            }

            // Get configurations (hardcoded for now)
            double minChurnProb = 0.2;
            double minInterventionScore = 0.35;
            var excludedSegments = new List<string> { "Churned", "Blacklisted" };
            double highPriorityThreshold = 0.75;

            // Check eligibility
            if (profile.ChurnRiskScore < minChurnProb)
            {
                response.Explanation.Add($"Churn risk score {profile.ChurnRiskScore} is below minimum {minChurnProb}.");
                return response;
            }

            if (excludedSegments.Contains(profile.CustomerType))
            {
                response.Explanation.Add($"Customer type {profile.CustomerType} is excluded from interventions.");
                return response;
            }

            // Calculate intervention score (simplified heuristic)
            double churnWeight = 0.3;
            double engagementWeight = 0.2;
            double segmentWeight = 0.15;
            double recencyWeight = 0.1;

            // Map segment to score (e.g., Vocal Churn higher)
            double segmentScore = profile.CustomerType == "Vocal Churn" ? 1.0 : profile.CustomerType == "Healthy" ? 0.2 : 0.5;
            // Recency based on recharge_change_pct (positive change means recent activity)
            double recencyScore = Math.Max(0, Math.Min(1, (profile.RechargeChangePct + 1) / 2)); // Normalize roughly

            double interventionScore = (profile.ChurnRiskScore * churnWeight) +
                                       (profile.EngagementScore * engagementWeight) +
                                       (segmentScore * segmentWeight) +
                                       (recencyScore * recencyWeight);

            response.InterventionScore = interventionScore;

            if (interventionScore < minInterventionScore)
            {
                response.Explanation.Add($"Intervention score {interventionScore} is below minimum {minInterventionScore}.");
                return response;
            }

            response.ShouldIntervene = true;

            // Determine priority
            if (interventionScore >= highPriorityThreshold)
            {
                response.Priority = "High";
            }
            else if (interventionScore >= 0.5)
            {
                response.Priority = "Medium";
            }
            else
            {
                response.Priority = "Low";
            }

            // Select offer
            var offerValues = new Dictionary<string, int>
            {
                ["DataBundle"] = 1000,
                ["AirtimeBonus"] = 500,
                ["VoicePlan"] = 800,
                ["ComboPlan"] = 1500,
                ["LoyaltyReward"] = 2000,
                ["WinBack"] = 3000,
                ["DeviceOffer"] = 5000,
                ["RoamingPack"] = 1200
            };
            if (profile.CustomerType == "Vocal Churn")
            {
                response.SelectedOffer = "WinBack";
            }
            else if (profile.ChurnRiskScore > 0.7)
            {
                response.SelectedOffer = "LoyaltyReward";
            }
            else
            {
                response.SelectedOffer = "DataBundle";
            }

            // Choose delivery channel
            response.DeliveryChannel = await _channelService.SelectChannelAsync(profile.CustomerType) ?? "SMS";

            // Evaluate execution mode
            response.ExecutionMode = await _policyService.EvaluateExecutionModeAsync(profile.ChurnRiskScore, profile.CustomerType);

            // If manual, queue for approval
            if (response.ExecutionMode == "Manual")
            {
                await _approvalService.QueueInterventionAsync(customerId, response.SelectedOffer, response.DeliveryChannel);
            }

            // Choose timing
            var now = DateTime.Now;
            var startHour = 8;
            var endHour = 20;
            if (now.Hour >= startHour && now.Hour <= endHour)
            {
                response.Timing = "Immediate";
            }
            else
            {
                response.Timing = $"Scheduled for {startHour}:00";
            }

            // Explanation
            response.Explanation.Add($"Churn risk: {profile.ChurnRiskScore}");
            response.Explanation.Add($"Engagement: {profile.EngagementScore}");
            response.Explanation.Add($"Segment: {profile.CustomerType}");
            response.Explanation.Add($"Calculated score: {interventionScore}");

            return response;
        }

        private async Task<bool> IsEligibleAsync(CustomerRiskProfile profile, OfferConfigurationDTO offer)
        {
            var rules = await _offerService.GetEligibilityRulesByOfferIdAsync(offer.Id);
            foreach (var rule in rules)
            {
                if (!MatchesRule(profile, rule))
                {
                    return false;
                }
            }
            return true;
        }

        private bool MatchesRule(CustomerRiskProfile profile, OfferEligibilityRuleDTO rule)
        {
            if (!string.IsNullOrEmpty(rule.ChurnClassification) && profile.CustomerType != rule.ChurnClassification)
                return false;
            if (rule.MinChurnRisk.HasValue && profile.ChurnRiskScore < rule.MinChurnRisk.Value)
                return false;
            if (rule.MaxChurnRisk.HasValue && profile.ChurnRiskScore > rule.MaxChurnRisk.Value)
                return false;
            if (rule.MinEngagement.HasValue && profile.EngagementScore < rule.MinEngagement.Value)
                return false;
            if (rule.MaxEngagement.HasValue && profile.EngagementScore > rule.MaxEngagement.Value)
                return false;
            if (rule.HasComplaint.HasValue && profile.HasComplaint != rule.HasComplaint.Value)
                return false;
            if (rule.MinNetworkQuality.HasValue && profile.NetworkQuality < rule.MinNetworkQuality.Value)
                return false;
            if (rule.MaxNetworkQuality.HasValue && profile.NetworkQuality > rule.MaxNetworkQuality.Value)
                return false;
            return true;
        }
    }
}