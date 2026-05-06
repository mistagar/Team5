using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team5Hackathon.Domain.Entities;

namespace Team5Hackathon.Domain.RepositoriesContract
{
    public interface IInterventionQueueRepository
    {
        Task<InterventionQueue?> GetByIdAsync(Guid id);
        Task<IEnumerable<InterventionQueue>> GetPendingAsync();
        Task<IEnumerable<InterventionQueue>> GetByCustomerIdAsync(string customerId);
        Task<IEnumerable<InterventionQueue>> GetHistoryAsync();
        Task<InterventionQueue> CreateAsync(InterventionQueue queue);
        Task<bool> UpdateAsync(InterventionQueue queue);
        Task<bool> DeleteAsync(Guid id);
    }
}