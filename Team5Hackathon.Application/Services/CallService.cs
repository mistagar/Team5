using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Team5Hackathon.Application.DTOs;
using Team5Hackathon.Application.DTOs.UserDTO;
using Team5Hackathon.Application.Services;
using Team5Hackathon.Domain.Entities;
using Team5Hackathon.Domain.RepositoriesContract;

namespace Team5Hackathon.Application.Services
{
    public class CallService : ICallService
    {
        private readonly ICallRepository _callRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAIService _aiService;
        private readonly ILogger<CallService> _logger;
        private readonly IUserService _userService;

        public CallService(
            ICallRepository callRepository,
            IUserRepository userRepository,
            IAIService aiService,
            ILogger<CallService> logger,
            IUserService userService)
        {
            _callRepository = callRepository;
            _userRepository = userRepository;
            _aiService = aiService;
            _logger = logger;
            _userService = userService;
        }

        public async Task<CallDTO?> StartCallAsync(StartCallDTO dto)
        {
            // 1. Create the UserRequest first (status: pending, type: "call")
            var userRequest = new UserRequest
            {
                Id = Guid.NewGuid(),
                UserId = dto.ClientId,
                Type = "call",
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };
            await _userRepository.CreateUserRequestAsync(userRequest);

            // 2. Create the Call and link it to the UserRequest
            var call = new Call
            {
                Id = Guid.NewGuid(),
                UserId = dto.ClientId,
                RequestId = userRequest.Id,
                StartTime = DateTime.UtcNow,
                Status = "active"
            };
            var createdCall = await _callRepository.CreateCallAsync(call);

            // 3. Write the CallId back onto the UserRequest so both sides are linked
            userRequest.CallId = createdCall.Id;
            await _userRepository.UpdateUserRequestAsync(userRequest);

            _logger.LogInformation(
                "Call {CallId} started for client {ClientId}. Linked to UserRequest {RequestId}",
                createdCall.Id, dto.ClientId, userRequest.Id);

            return MapToCallDTO(createdCall);
        }

        public async Task<bool> EndCallAsync(EndCallDTO dto)
        {
            var call = await _callRepository.GetCallByIdAsync(dto.CallId);
            if (call == null) return false;

            call.EndTime = DateTime.UtcNow;
            call.Transcript = dto.Transcript;
            call.SatisfactionRating = dto.SatisfactionRating;
            call.IsResolved = dto.IsResolved;
            call.Status = "ended";

            if (!string.IsNullOrEmpty(dto.Transcript))
            {
                // AI analysis — fills category, sentiment, summary in one shot
                var analysis = await _aiService.AnalyzeComplaintAsync(dto.Transcript);
                call.Summary       = string.IsNullOrWhiteSpace(analysis.Summary) ? await _aiService.GenerateSummaryAsync(dto.Transcript) : analysis.Summary;
                call.ActionItems   = await _aiService.GenerateActionItemsAsync(dto.Transcript);
                call.PrimaryIntent = analysis.Category;
                call.Category      = analysis.Category;
                call.Sentiment     = analysis.Sentiment;

                // Update the linked UserRequest with AI results
                if (call.RequestId.HasValue)
                {
                    var userRequest = await _userRepository.GetUserRequestByIdAsync(call.RequestId.Value);
                    if (userRequest != null)
                    {
                        userRequest.Content   = dto.Transcript;
                        userRequest.Summary   = call.Summary;
                        userRequest.Response  = analysis.EnglishResponse;
                        userRequest.Category  = analysis.Category;
                        userRequest.Sentiment = analysis.Sentiment;
                        userRequest.Status    = dto.IsResolved ? "resolved" : "processed";
                        await _userRepository.UpdateUserRequestAsync(userRequest);

                        _logger.LogInformation(
                            "UserRequest {RequestId} updated: category={Category}, sentiment={Sentiment}, status={Status}",
                            userRequest.Id, userRequest.Category, userRequest.Sentiment, userRequest.Status);
                    }
                }
            }

            var result = await _callRepository.UpdateCallAsync(call);
            _logger.LogInformation("Call {CallId} ended", dto.CallId);
            return result;
        }

        public async Task<bool> AddTranscriptSegmentAsync(TranscriptSegmentDTO dto)
        {
            var segment = new TranscriptSegment
            {
                Id = Guid.NewGuid(),
                CallId = dto.CallId,
                Timestamp = dto.Timestamp,
                Text = dto.Text
            };
            segment.Intent = await _aiService.ExtractIntentAsync(dto.Text ?? string.Empty);
            await _callRepository.AddTranscriptSegmentAsync(segment);
            _logger.LogInformation("Transcript segment added for call {CallId}", dto.CallId);
            return true;
        }

        public async Task<CallDTO?> GetCallByIdAsync(Guid callId)
        {
            var call = await _callRepository.GetCallByIdAsync(callId);
            return call == null ? null : MapToCallDTO(call);
        }

        public async Task<IEnumerable<CallDTO>> GetCallsByClientIdAsync(Guid clientId)
        {
            var calls = await _callRepository.GetCallsByClientIdAsync(clientId);
            return calls.Select(MapToCallDTO);
        }

        public async Task<FollowUpMessageDTO> GenerateFollowUpMessageAsync(GenerateFollowUpDTO dto)
        {
            var call = await _callRepository.GetCallByIdAsync(dto.CallId);
            if (call == null) throw new Exception("Call not found");
            var content = await _aiService.GenerateFollowUpContentAsync(call.PrimaryIntent ?? "", call.Summary ?? "");
            var message = new FollowUpMessage
            {
                Id = Guid.NewGuid(),
                CallId = dto.CallId,
                Type = dto.Type,
                Content = content,
                IsApproved = false
            };
            var added = await _callRepository.AddFollowUpMessageAsync(message);
            return new FollowUpMessageDTO
            {
                Id = added.Id,
                CallId = added.CallId,
                Type = added.Type,
                Content = added.Content,
                IsApproved = added.IsApproved,
                SentAt = added.SentAt,
                DeliveryStatus = added.DeliveryStatus
            };
        }

        public async Task<bool> ApproveFollowUpMessageAsync(Guid messageId)
        {
            _logger.LogInformation("Follow-up message approved {MessageId}", messageId);
            return true;
        }

        public async Task<bool> SendFollowUpMessageAsync(Guid messageId)
        {
            _logger.LogInformation("Follow-up message sent {MessageId}", messageId);
            return true;
        }

        public async Task<DashboardMetricsDTO> GetDashboardMetricsAsync()
        {
            // Execute queries sequentially to avoid concurrent DbContext access
            var totalClients = await _callRepository.GetTotalClientsAttendedAsync();
            var resolved     = await _callRepository.GetIssuesResolvedAsync();
            var pending      = await _callRepository.GetIssuesPendingAsync();
            var avgRating    = await _callRepository.GetAverageSatisfactionRatingAsync();
            
            return new DashboardMetricsDTO
            {
                TotalClientsAttended    = totalClients,
                IssuesResolved          = resolved,
                IssuesPending           = pending,
                AverageSatisfactionRating = avgRating
            };
        }

        public async Task<ClientDashboardDTO> GetClientDashboardAsync(Guid clientId)
        {
            var calls    = await GetCallsByClientIdAsync(clientId);
            var requests = await _userService.GetUserRequestsAsync(clientId);
            return new ClientDashboardDTO
            {
                CallHistory    = calls,
                RequestHistory = requests
            };
        }

        public async Task<AnalyticsDashboardDTO> GetAnalyticsDashboardAsync(int dailyVolumeDays = 30)
        {
            
            var totalCalls    = await _callRepository.GetTotalCallsAsync(dailyVolumeDays);
            var activeCalls   = await _callRepository.GetActiveCallsAsync(); 
            var endedCalls    = await _callRepository.GetEndedCallsAsync(); 
            var uniqueClients = await _callRepository.GetTotalClientsAttendedAsync(); 
            var resolved      = await _callRepository.GetIssuesResolvedAsync(dailyVolumeDays);
            var pending       = await _callRepository.GetIssuesPendingAsync(dailyVolumeDays);
            var avgRating     = await _callRepository.GetAverageSatisfactionRatingAsync(dailyVolumeDays);
            var byCategory    = await _callRepository.GetCallsByCategoryAsync(dailyVolumeDays);
            var bySentiment   = await _callRepository.GetCallsBySentimentAsync(dailyVolumeDays);
            var unresolvedCat = await _callRepository.GetUnresolvedByCategoryAsync(dailyVolumeDays);
            var dailyVolume   = await _callRepository.GetDailyCallVolumeAsync(dailyVolumeDays);
            var avgDuration   = await _callRepository.GetAverageCallDurationAsync(dailyVolumeDays);

            var allCategories = new[] { "network", "data", "billing", "call", "sim", "other" };
            var allSentiments = new[] { "positive", "neutral", "frustrated", "angry" };
            foreach (var cat in allCategories) byCategory.TryAdd(cat, 0);
            foreach (var s   in allSentiments) bySentiment.TryAdd(s, 0);

            double ToPercent(int count) =>
                totalCalls > 0 ? Math.Round(count * 100.0 / totalCalls, 1) : 0;

            return new AnalyticsDashboardDTO
            {
                TotalCalls                = totalCalls,
                ActiveCalls               = activeCalls,
                EndedCalls                = endedCalls,
                TotalUniqueClients        = uniqueClients,
                IssuesResolved            = resolved,
                IssuesPending             = pending,
                ResolutionRate            = totalCalls > 0 ? Math.Round(resolved * 100.0 / totalCalls, 1) : 0,
                AverageSatisfactionRating = avgRating,
                CallsByCategory           = byCategory,
                CategoryPercentages       = byCategory.ToDictionary(kv => kv.Key, kv => ToPercent(kv.Value)),
                CallsBySentiment          = bySentiment,
                SentimentPercentages      = bySentiment.ToDictionary(kv => kv.Key, kv => ToPercent(kv.Value)),
                DailyCallVolume           = dailyVolume,
                UnresolvedByCategory      = unresolvedCat,
                AverageCallDurationMinutes = avgDuration
            };
        }

     
        private static CallDTO MapToCallDTO(Call c) => new()
        {
            Id                = c.Id,
            ClientId          = c.UserId,
            RequestId         = c.RequestId,
            StartTime         = c.StartTime,
            EndTime           = c.EndTime,
            Transcript        = c.Transcript,
            Summary           = c.Summary,
            ActionItems       = c.ActionItems,
            PrimaryIntent     = c.PrimaryIntent,
            Category          = c.Category,
            Sentiment         = c.Sentiment,
            SatisfactionRating = c.SatisfactionRating,
            IsResolved        = c.IsResolved,
            Status            = c.Status
        };
    }
}