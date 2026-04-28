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
    public class CallRepository : ICallRepository
    {
        private readonly AppDbContext _context;

        public CallRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Call?> GetCallByIdAsync(Guid callId)
        {
            return await _context.Calls.FindAsync(callId);
        }

        public async Task<IEnumerable<Call>> GetCallsByClientIdAsync(Guid clientId)
        {
            return await _context.Calls.Where(c => c.UserId == clientId).ToListAsync();
        }

        public async Task<Call> CreateCallAsync(Call call)
        {
            _context.Calls.Add(call);
            await _context.SaveChangesAsync();
            return call;
        }

        public async Task<bool> UpdateCallAsync(Call call)
        {
            _context.Calls.Update(call);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCallAsync(Guid callId)
        {
            var call = await GetCallByIdAsync(callId);
            if (call == null) return false;
            _context.Calls.Remove(call);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<TranscriptSegment>> GetTranscriptSegmentsByCallIdAsync(Guid callId)
        {
            return await _context.TranscriptSegments.Where(ts => ts.CallId == callId).ToListAsync();
        }

        public async Task<TranscriptSegment> AddTranscriptSegmentAsync(TranscriptSegment segment)
        {
            _context.TranscriptSegments.Add(segment);
            await _context.SaveChangesAsync();
            return segment;
        }

        public async Task<IEnumerable<FollowUpMessage>> GetFollowUpMessagesByCallIdAsync(Guid callId)
        {
            return await _context.FollowUpMessages.Where(fm => fm.CallId == callId).ToListAsync();
        }

        public async Task<FollowUpMessage> AddFollowUpMessageAsync(FollowUpMessage message)
        {
            _context.FollowUpMessages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<bool> UpdateFollowUpMessageAsync(FollowUpMessage message)
        {
            _context.FollowUpMessages.Update(message);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> GetTotalClientsAttendedAsync()
        {
            return await _context.Calls.Select(c => c.UserId).Distinct().CountAsync();
        }

        public async Task<int> GetIssuesResolvedAsync()
        {
            return await _context.Calls.CountAsync(c => c.IsResolved);
        }

        public async Task<int> GetIssuesPendingAsync()
        {
            return await _context.Calls.CountAsync(c => !c.IsResolved && c.Status == "ended");
        }

        public async Task<double?> GetAverageSatisfactionRatingAsync()
        {
            return await _context.Calls.Where(c => c.SatisfactionRating.HasValue).AverageAsync(c => c.SatisfactionRating);
        }
    }
}