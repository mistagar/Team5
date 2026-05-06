using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team5Hackathon.Domain.Entities;

namespace Team5Hackathon.Domain.RepositoriesContract
{
    public interface IChannelRepository
    {
        Task<ChannelConfiguration?> GetChannelByIdAsync(Guid channelId);
        Task<IEnumerable<ChannelConfiguration>> GetAllChannelsAsync();
        Task<IEnumerable<ChannelConfiguration>> GetEnabledChannelsAsync();
        Task<ChannelConfiguration> CreateChannelAsync(ChannelConfiguration channel);
        Task<bool> UpdateChannelAsync(ChannelConfiguration channel);
        Task<bool> DeleteChannelAsync(Guid channelId);
        Task<ChannelConfiguration?> GetChannelByTypeAsync(string channelType);
    }
}