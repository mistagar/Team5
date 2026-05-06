using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Team5Hackathon.API.DTOs;
using Team5Hackathon.Application.DTOs;
using Team5Hackathon.Application.Services;

namespace Team5Hackathon.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ChannelController : ControllerBase
    {
        private readonly IChannelManagementService _channelService;

        public ChannelController(IChannelManagementService channelService)
        {
            _channelService = channelService;
        }

        [HttpGet("{channelId}")]
        [ProducesResponseType(typeof(ApiResponse<ChannelConfigurationDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetChannel(Guid channelId)
        {
            try
            {
                var channel = await _channelService.GetChannelByIdAsync(channelId);
                if (channel == null) return NotFound(ApiResponse<string>.FailResponse("Channel not found"));
                return Ok(ApiResponse<ChannelConfigurationDTO>.SuccessResponse(channel));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching channel", new List<string> { ex.Message }));
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ChannelConfigurationDTO>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllChannels()
        {
            try
            {
                var channels = await _channelService.GetAllChannelsAsync();
                return Ok(ApiResponse<IEnumerable<ChannelConfigurationDTO>>.SuccessResponse(channels));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching channels", new List<string> { ex.Message }));
            }
        }

        [HttpGet("enabled")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ChannelConfigurationDTO>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetEnabledChannels()
        {
            try
            {
                var channels = await _channelService.GetEnabledChannelsAsync();
                return Ok(ApiResponse<IEnumerable<ChannelConfigurationDTO>>.SuccessResponse(channels));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching enabled channels", new List<string> { ex.Message }));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ChannelConfigurationDTO>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateChannel([FromBody] CreateChannelDTO dto)
        {
            try
            {
                var channel = await _channelService.CreateChannelAsync(dto);
                return CreatedAtAction(nameof(GetChannel), new { channelId = channel.Id }, ApiResponse<ChannelConfigurationDTO>.SuccessResponse(channel, "Channel created"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.FailResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error creating channel", new List<string> { ex.Message }));
            }
        }

        [HttpPut("{channelId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateChannel(Guid channelId, [FromBody] UpdateChannelDTO dto)
        {
            try
            {
                var success = await _channelService.UpdateChannelAsync(channelId, dto);
                if (!success) return NotFound(ApiResponse<string>.FailResponse("Channel not found"));
                return Ok(ApiResponse<string>.SuccessResponse("Channel updated"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.FailResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error updating channel", new List<string> { ex.Message }));
            }
        }

        [HttpDelete("{channelId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteChannel(Guid channelId)
        {
            try
            {
                var success = await _channelService.DeleteChannelAsync(channelId);
                if (!success) return NotFound(ApiResponse<string>.FailResponse("Channel not found"));
                return Ok(ApiResponse<string>.SuccessResponse("Channel deleted"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error deleting channel", new List<string> { ex.Message }));
            }
        }

        [HttpGet("select/{churnClassification}")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SelectChannel(string churnClassification)
        {
            try
            {
                var channel = await _channelService.SelectChannelAsync(churnClassification);
                if (channel == null) return NotFound(ApiResponse<string>.FailResponse("No suitable channel found"));
                return Ok(ApiResponse<string>.SuccessResponse(channel));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error selecting channel", new List<string> { ex.Message }));
            }
        }
    }
}