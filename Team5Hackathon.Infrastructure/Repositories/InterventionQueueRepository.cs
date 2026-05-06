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
    public class InterventionQueueRepository : IInterventionQueueRepository
    {
        private readonly AppDbContext _context;

        public InterventionQueueRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<InterventionQueue?> GetByIdAsync(Guid id)
        {
            return await _context.InterventionQueues.FindAsync(id);
        }

        public async Task<IEnumerable<InterventionQueue>> GetPendingAsync()
        {
            return await _context.InterventionQueues.Where(q => q.Status == "Pending").ToListAsync();
        }

        public async Task<IEnumerable<InterventionQueue>> GetByCustomerIdAsync(string customerId)
        {
            return await _context.InterventionQueues.Where(q => q.CustomerId == customerId).ToListAsync();
        }

        public async Task<IEnumerable<InterventionQueue>> GetHistoryAsync()
        {
            return await _context.InterventionQueues.Where(q => q.Status != "Pending").ToListAsync();
        }

        public async Task<InterventionQueue> CreateAsync(InterventionQueue queue)
        {
            queue.CreatedAt = DateTime.UtcNow;
            _context.InterventionQueues.Add(queue);
            await _context.SaveChangesAsync();
            return queue;
        }

        public async Task<bool> UpdateAsync(InterventionQueue queue)
        {
            _context.InterventionQueues.Update(queue);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var queue = await _context.InterventionQueues.FindAsync(id);
            if (queue == null) return false;
            _context.InterventionQueues.Remove(queue);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}