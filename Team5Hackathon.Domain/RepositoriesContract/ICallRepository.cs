using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team5Hackathon.Domain.Entities;

namespace Team5Hackathon.Domain.RepositoriesContract
{
    public interface ICallRepository
    {
        Task<Call?> GetCallByIdAsync(Guid callId);
        Task<IEnumerable<Call>> GetCallsByClientIdAsync(Guid clientId);
        Task<Call> CreateCallAsync(Call call);
        Task<bool> UpdateCallAsync(Call call);
        Task<bool> DeleteCallAsync(Guid callId);
        Task<IEnumerable<TranscriptSegment>> GetTranscriptSegmentsByCallIdAsync(Guid callId);
        Task<TranscriptSegment> AddTranscriptSegmentAsync(TranscriptSegment segment);
        Task<IEnumerable<FollowUpMessage>> GetFollowUpMessagesByCallIdAsync(Guid callId);
        Task<FollowUpMessage> AddFollowUpMessageAsync(FollowUpMessage message);
        Task<bool> UpdateFollowUpMessageAsync(FollowUpMessage message);
        // For dashboard metrics
        Task<int> GetTotalClientsAttendedAsync();
        Task<int> GetIssuesResolvedAsync();
        Task<int> GetIssuesPendingAsync();
        Task<double?> GetAverageSatisfactionRatingAsync();
    }
}