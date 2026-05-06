using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team5Hackathon.Application.DTOs;

namespace Team5Hackathon.Application.Services
{
    public interface IChannelManagementService
    {
        Task<ChannelConfigurationDTO?> GetChannelByIdAsync(Guid channelId);
        Task<IEnumerable<ChannelConfigurationDTO>> GetAllChannelsAsync();
        Task<IEnumerable<ChannelConfigurationDTO>> GetEnabledChannelsAsync();
        Task<ChannelConfigurationDTO> CreateChannelAsync(CreateChannelDTO dto);
        Task<bool> UpdateChannelAsync(Guid channelId, UpdateChannelDTO dto);
        Task<bool> DeleteChannelAsync(Guid channelId);
        Task<string?> SelectChannelAsync(string churnClassification);
    }
}