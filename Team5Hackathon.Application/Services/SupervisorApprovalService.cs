using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Team5Hackathon.Application.DTOs;
using Team5Hackathon.Application.DTOs.Audit;
using Team5Hackathon.Application.Services;
using Team5Hackathon.Domain.Entities;
using Team5Hackathon.Domain.RepositoriesContract;

namespace Team5Hackathon.Application.Services
{
    public class SupervisorApprovalService : ISupervisorApprovalService
    {
        private readonly IInterventionQueueRepository _queueRepository;
        private readonly IAuditService _auditService;

        public SupervisorApprovalService(IInterventionQueueRepository queueRepository, IAuditService auditService)
        {
            _queueRepository = queueRepository;
            _auditService = auditService;
        }

        public async Task<InterventionQueueDTO> QueueInterventionAsync(string customerId, string recommendedOffer, string recommendedChannel)
        {
            var queue = new InterventionQueue
            {
                CustomerId = customerId,
                RecommendedOffer = recommendedOffer,
                RecommendedChannel = recommendedChannel
            };

            var created = await _queueRepository.CreateAsync(queue);
            return MapToDTO(created);
        }

        public async Task<IEnumerable<InterventionQueueDTO>> GetPendingApprovalsAsync()
        {
            var pending = await _queueRepository.GetPendingAsync();
            return pending.Select(MapToDTO);
        }

        public async Task<InterventionQueueDTO?> GetApprovalByIdAsync(Guid id)
        {
            var queue = await _queueRepository.GetByIdAsync(id);
            return queue == null ? null : MapToDTO(queue);
        }

        public async Task<bool> ApproveInterventionAsync(Guid id, Guid approvedBy, string? modifiedOffer, string? modifiedChannel, string? notes)
        {
            var queue = await _queueRepository.GetByIdAsync(id);
            if (queue == null || queue.Status != "Pending") return false;

            queue.Status = "Approved";
            queue.ApprovedBy = approvedBy;
            queue.ApprovedOffer = modifiedOffer ?? queue.RecommendedOffer;
            queue.ApprovedChannel = modifiedChannel ?? queue.RecommendedChannel;
            queue.ApprovalNotes = notes;
            queue.ApprovedAt = DateTime.UtcNow;

            var success = await _queueRepository.UpdateAsync(queue);
            if (success)
            {
                await _auditService.RecordAsync(new AuditEntry
                {
                    Action = "InterventionApproved",
                    Outcome = "Success",
                    Description = $"Intervention {id} approved by {approvedBy}",
                    PerformedById = approvedBy.ToString(),
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
            return success;
        }

        public async Task<bool> RejectInterventionAsync(Guid id, Guid rejectedBy, string notes)
        {
            var queue = await _queueRepository.GetByIdAsync(id);
            if (queue == null || queue.Status != "Pending") return false;

            queue.Status = "Rejected";
            queue.ApprovedBy = rejectedBy;
            queue.ApprovalNotes = notes;
            queue.ApprovedAt = DateTime.UtcNow;

            var success = await _queueRepository.UpdateAsync(queue);
            if (success)
            {
                await _auditService.RecordAsync(new AuditEntry
                {
                    Action = "InterventionRejected",
                    Outcome = "Success",
                    Description = $"Intervention {id} rejected by {rejectedBy}",
                    PerformedById = rejectedBy.ToString(),
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
            return success;
        }

        public async Task<bool> ExecuteInterventionAsync(Guid id)
        {
            var queue = await _queueRepository.GetByIdAsync(id);
            if (queue == null || queue.Status != "Approved") return false;

            queue.Status = "Executed";
            queue.ExecutedAt = DateTime.UtcNow;

            var success = await _queueRepository.UpdateAsync(queue);
            if (success)
            {
                await _auditService.RecordAsync(new AuditEntry
                {
                    Action = "InterventionExecuted",
                    Outcome = "Success",
                    Description = $"Intervention {id} executed",
                    PerformedById = queue.ApprovedBy?.ToString(),
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
            return success;
        }

        public async Task<IEnumerable<InterventionQueueDTO>> GetApprovalHistoryAsync()
        {
            var history = await _queueRepository.GetHistoryAsync();
            return history.Select(MapToDTO);
        }

        public async Task<IEnumerable<InterventionQueueDTO>> GetByCustomerIdAsync(string customerId)
        {
            var queues = await _queueRepository.GetByCustomerIdAsync(customerId);
            return queues.Select(MapToDTO);
        }

        private InterventionQueueDTO MapToDTO(InterventionQueue queue)
        {
            return new InterventionQueueDTO
            {
                Id = queue.Id,
                CustomerId = queue.CustomerId,
                RecommendedOffer = queue.RecommendedOffer,
                RecommendedChannel = queue.RecommendedChannel,
                Status = queue.Status,
                ApprovedOffer = queue.ApprovedOffer,
                ApprovedChannel = queue.ApprovedChannel,
                ApprovedBy = queue.ApprovedBy,
                ApprovalNotes = queue.ApprovalNotes,
                CreatedAt = queue.CreatedAt,
                ApprovedAt = queue.ApprovedAt,
                ExecutedAt = queue.ExecutedAt
            };
        }
    }
}