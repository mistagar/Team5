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

        public async Task<int> GetIssuesResolvedAsync(int days)
        {
            var since = DateTime.UtcNow.Date.AddDays(-days + 1);
            return await _context.Calls.CountAsync(c => c.IsResolved && c.StartTime >= since);
        }

        public async Task<int> GetIssuesPendingAsync()
        {
            return await _context.Calls.CountAsync(c => !c.IsResolved && c.Status == "ended");
        }

        public async Task<int> GetIssuesPendingAsync(int days)
        {
            var since = DateTime.UtcNow.Date.AddDays(-days + 1);
            return await _context.Calls.CountAsync(c => !c.IsResolved && c.Status == "ended" && c.StartTime >= since);
        }

        public async Task<double?> GetAverageSatisfactionRatingAsync()
        {
            return await _context.Calls.Where(c => c.SatisfactionRating.HasValue).AverageAsync(c => c.SatisfactionRating);
        }

        public async Task<double?> GetAverageSatisfactionRatingAsync(int days)
        {
            var since = DateTime.UtcNow.Date.AddDays(-days + 1);
            return await _context.Calls
                .Where(c => c.SatisfactionRating.HasValue && c.StartTime >= since)
                .AverageAsync(c => c.SatisfactionRating);
        }

        public async Task<int> GetTotalCallsAsync()
        {
            return await _context.Calls.CountAsync();
        }

        public async Task<int> GetTotalCallsAsync(int days)
        {
            var since = DateTime.UtcNow.Date.AddDays(-days + 1);
            return await _context.Calls.CountAsync(c => c.StartTime >= since);
        }

        public async Task<int> GetActiveCallsAsync()
        {
            return await _context.Calls.CountAsync(c => c.Status == "active");
        }

        public async Task<int> GetEndedCallsAsync()
        {
            return await _context.Calls.CountAsync(c => c.Status == "ended");
        }

        public async Task<Dictionary<string, int>> GetCallsByCategoryAsync()
        {
            return await _context.Calls
                .Where(c => c.Category != null)
                .GroupBy(c => c.Category!)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetCallsByCategoryAsync(int days)
        {
            var since = DateTime.UtcNow.Date.AddDays(-days + 1);
            return await _context.Calls
                .Where(c => c.Category != null && c.StartTime >= since)
                .GroupBy(c => c.Category!)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetCallsBySentimentAsync()
        {
            return await _context.Calls
                .Where(c => c.Sentiment != null)
                .GroupBy(c => c.Sentiment!)
                .Select(g => new { Sentiment = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Sentiment, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetCallsBySentimentAsync(int days)
        {
            var since = DateTime.UtcNow.Date.AddDays(-days + 1);
            return await _context.Calls
                .Where(c => c.Sentiment != null && c.StartTime >= since)
                .GroupBy(c => c.Sentiment!)
                .Select(g => new { Sentiment = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Sentiment, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetUnresolvedByCategoryAsync()
        {
            return await _context.Calls
                .Where(c => !c.IsResolved && c.Category != null && c.Status == "ended")
                .GroupBy(c => c.Category!)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetUnresolvedByCategoryAsync(int days)
        {
            var since = DateTime.UtcNow.Date.AddDays(-days + 1);
            return await _context.Calls
                .Where(c => !c.IsResolved && c.Category != null && c.Status == "ended" && c.StartTime >= since)
                .GroupBy(c => c.Category!)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetDailyCallVolumeAsync(int days)
        {
            // For last 30 days excluding today (29 days ago to yesterday)
            var since = DateTime.UtcNow.Date.AddDays(-days + 1);
            var raw = await _context.Calls
                .Where(c => c.StartTime >= since)
                .GroupBy(c => c.StartTime.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();

            // Fill in zeros for days with no calls so the chart has a continuous series
            return Enumerable.Range(0, days)
                .Select(i => since.AddDays(i))
                .ToDictionary(
                    date => date.ToString("yyyy-MM-dd"),
                    date => raw.FirstOrDefault(r => r.Date == date)?.Count ?? 0);
        }

        //public async Task<Dictionary<string, int>> GetDailyCallVolumeAsync(int days)
        //{
        //    var since = DateTime.UtcNow.Date.AddDays(-days + 1);
        //    var raw = await _context.Calls
        //        .Where(c => c.StartTime >= since)
        //        .GroupBy(c => c.StartTime.Date)
        //        .Select(g => new { Date = g.Key, Count = g.Count() })
        //        .ToListAsync();

        //    // Fill in zeros for days with no calls so the chart has a continuous series
        //    return Enumerable.Range(0, days)
        //        .Select(i => since.AddDays(i))
        //        .ToDictionary(
        //            date => date.ToString("yyyy-MM-dd"),
        //            date => raw.FirstOrDefault(r => r.Date == date)?.Count ?? 0);
        //}

        public async Task<double?> GetAverageCallDurationAsync()
        {
            var durations = await _context.Calls
                .Where(c => c.Status == "ended" && c.EndTime.HasValue)
                .Select(c => EF.Functions.DateDiffMinute(c.StartTime, c.EndTime!.Value))
                .ToListAsync();

            return durations.Count > 0 ? durations.Average(d => (double)d) : null;
        }

        public async Task<double?> GetAverageCallDurationAsync(int days)
        {
            var since = DateTime.UtcNow.Date.AddDays(-days + 1);
            var durations = await _context.Calls
                .Where(c => c.Status == "ended" && c.EndTime.HasValue && c.StartTime >= since)
                .Select(c => EF.Functions.DateDiffMinute(c.StartTime, c.EndTime!.Value))
                .ToListAsync();

            return durations.Count > 0 ? durations.Average(d => (double)d) : null;
        }
    }
}