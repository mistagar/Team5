using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team5Hackathon.Application.DTOs;

namespace Team5Hackathon.Application.Services
{
    public interface ISupervisorApprovalService
    {
        Task<InterventionQueueDTO> QueueInterventionAsync(string customerId, string recommendedOffer, string recommendedChannel);
        Task<IEnumerable<InterventionQueueDTO>> GetPendingApprovalsAsync();
        Task<InterventionQueueDTO?> GetApprovalByIdAsync(Guid id);
        Task<bool> ApproveInterventionAsync(Guid id, Guid approvedBy, string? modifiedOffer, string? modifiedChannel, string? notes);
        Task<bool> RejectInterventionAsync(Guid id, Guid rejectedBy, string notes);
        Task<bool> ExecuteInterventionAsync(Guid id);
        Task<IEnumerable<InterventionQueueDTO>> GetApprovalHistoryAsync();
        Task<IEnumerable<InterventionQueueDTO>> GetByCustomerIdAsync(string customerId);
    }
}