using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team5Hackathon.Application.DTOs;
using Team5Hackathon.Application.Services;

namespace Team5Hackathon.Application.Services
{
    public interface ICallService
    {
        Task<CallDTO?> StartCallAsync(StartCallDTO dto);
        Task<bool> EndCallAsync(EndCallDTO dto);
        Task<bool> AddTranscriptSegmentAsync(TranscriptSegmentDTO dto);
        Task<CallDTO?> GetCallByIdAsync(Guid callId);
        Task<IEnumerable<CallDTO>> GetCallsByClientIdAsync(Guid clientId);
        Task<FollowUpMessageDTO> GenerateFollowUpMessageAsync(GenerateFollowUpDTO dto);
        Task<bool> ApproveFollowUpMessageAsync(Guid messageId);
        Task<bool> SendFollowUpMessageAsync(Guid messageId);
        Task<DashboardMetricsDTO> GetDashboardMetricsAsync();
        Task<ClientDashboardDTO> GetClientDashboardAsync(Guid clientId);
    }
}