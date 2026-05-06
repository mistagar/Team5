using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Team5Hackathon.Application.DTOs;
using Team5Hackathon.Application.Services;
using Team5Hackathon.Domain.Entities;
using Team5Hackathon.Domain.RepositoriesContract;

namespace Team5Hackathon.Application.Services
{
    public class ChannelManagementService : IChannelManagementService
    {
        private readonly IChannelRepository _channelRepository;

        public ChannelManagementService(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public async Task<ChannelConfigurationDTO?> GetChannelByIdAsync(Guid channelId)
        {
            var channel = await _channelRepository.GetChannelByIdAsync(channelId);
            return channel == null ? null : MapToDTO(channel);
        }

        public async Task<IEnumerable<ChannelConfigurationDTO>> GetAllChannelsAsync()
        {
            var channels = await _channelRepository.GetAllChannelsAsync();
            return channels.Select(MapToDTO);
        }

        public async Task<IEnumerable<ChannelConfigurationDTO>> GetEnabledChannelsAsync()
        {
            var channels = await _channelRepository.GetEnabledChannelsAsync();
            return channels.Select(MapToDTO);
        }

        public async Task<ChannelConfigurationDTO> CreateChannelAsync(CreateChannelDTO dto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(dto.ChannelType))
                throw new ArgumentException("Channel type is required.");
            var supported = new[] { "SMS", "USSD", "WhatsApp", "Push" };
            if (!supported.Contains(dto.ChannelType))
                throw new ArgumentException("Unsupported channel type.");
            if (dto.RetryCount < 0)
                throw new ArgumentException("Retry count must be non-negative.");
            if (dto.CooldownMinutes < 0)
                throw new ArgumentException("Cooldown minutes must be non-negative.");

            var existing = await _channelRepository.GetChannelByTypeAsync(dto.ChannelType);
            if (existing != null)
                throw new ArgumentException("Channel type already exists.");

            var channel = new ChannelConfiguration
            {
                ChannelType = dto.ChannelType,
                IsEnabled = dto.IsEnabled,
                RetryCount = dto.RetryCount,
                CooldownMinutes = dto.CooldownMinutes,
                PreferredChurnTypes = dto.PreferredChurnTypes
            };

            var created = await _channelRepository.CreateChannelAsync(channel);
            return MapToDTO(created);
        }

        public async Task<bool> UpdateChannelAsync(Guid channelId, UpdateChannelDTO dto)
        {
            var channel = await _channelRepository.GetChannelByIdAsync(channelId);
            if (channel == null) return false;

            // Validation
            if (string.IsNullOrWhiteSpace(dto.ChannelType))
                throw new ArgumentException("Channel type is required.");
            var supported = new[] { "SMS", "USSD", "WhatsApp", "Push" };
            if (!supported.Contains(dto.ChannelType))
                throw new ArgumentException("Unsupported channel type.");
            if (dto.RetryCount < 0)
                throw new ArgumentException("Retry count must be non-negative.");
            if (dto.CooldownMinutes < 0)
                throw new ArgumentException("Cooldown minutes must be non-negative.");

            var existing = await _channelRepository.GetChannelByTypeAsync(dto.ChannelType);
            if (existing != null && existing.Id != channelId)
                throw new ArgumentException("Channel type already exists.");

            channel.ChannelType = dto.ChannelType;
            channel.IsEnabled = dto.IsEnabled;
            channel.RetryCount = dto.RetryCount;
            channel.CooldownMinutes = dto.CooldownMinutes;
            channel.PreferredChurnTypes = dto.PreferredChurnTypes;

            return await _channelRepository.UpdateChannelAsync(channel);
        }

        public async Task<bool> DeleteChannelAsync(Guid channelId)
        {
            return await _channelRepository.DeleteChannelAsync(channelId);
        }

        public async Task<string?> SelectChannelAsync(string churnClassification)
        {
            var enabledChannels = await _channelRepository.GetEnabledChannelsAsync();
            var preferred = enabledChannels.FirstOrDefault(c =>
                !string.IsNullOrEmpty(c.PreferredChurnTypes) &&
                c.PreferredChurnTypes.Split(',').Contains(churnClassification.Trim()));

            if (preferred != null)
                return preferred.ChannelType;

            // Fallback to first enabled
            return enabledChannels.FirstOrDefault()?.ChannelType;
        }

        private ChannelConfigurationDTO MapToDTO(ChannelConfiguration channel)
        {
            return new ChannelConfigurationDTO
            {
                Id = channel.Id,
                ChannelType = channel.ChannelType,
                IsEnabled = channel.IsEnabled,
                RetryCount = channel.RetryCount,
                CooldownMinutes = channel.CooldownMinutes,
                PreferredChurnTypes = channel.PreferredChurnTypes,
                CreatedAt = channel.CreatedAt,
                UpdatedAt = channel.UpdatedAt
            };
        }
    }
}