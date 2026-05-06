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
    public class ChannelRepository : IChannelRepository
    {
        private readonly AppDbContext _context;

        public ChannelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ChannelConfiguration?> GetChannelByIdAsync(Guid channelId)
        {
            return await _context.ChannelConfigurations.FindAsync(channelId);
        }

        public async Task<IEnumerable<ChannelConfiguration>> GetAllChannelsAsync()
        {
            return await _context.ChannelConfigurations.ToListAsync();
        }

        public async Task<IEnumerable<ChannelConfiguration>> GetEnabledChannelsAsync()
        {
            return await _context.ChannelConfigurations.Where(c => c.IsEnabled).ToListAsync();
        }

        public async Task<ChannelConfiguration> CreateChannelAsync(ChannelConfiguration channel)
        {
            channel.CreatedAt = DateTime.UtcNow;
            _context.ChannelConfigurations.Add(channel);
            await _context.SaveChangesAsync();
            return channel;
        }

        public async Task<bool> UpdateChannelAsync(ChannelConfiguration channel)
        {
            channel.UpdatedAt = DateTime.UtcNow;
            _context.ChannelConfigurations.Update(channel);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteChannelAsync(Guid channelId)
        {
            var channel = await _context.ChannelConfigurations.FindAsync(channelId);
            if (channel == null) return false;
            _context.ChannelConfigurations.Remove(channel);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ChannelConfiguration?> GetChannelByTypeAsync(string channelType)
        {
            return await _context.ChannelConfigurations.FirstOrDefaultAsync(c => c.ChannelType == channelType);
        }
    }
}