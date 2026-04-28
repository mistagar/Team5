using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Team5Hackathon.API.DTOs;
using Team5Hackathon.Application.DTOs;
using Team5Hackathon.Application.Services;

namespace Team5Hackathon.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CallController : ControllerBase
    {
        private readonly ICallService _callService;

        public CallController(ICallService callService)
        {
            _callService = callService;
        }

        [HttpPost("start")]
        [ProducesResponseType(typeof(ApiResponse<CallDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> StartCall([FromBody] StartCallDTO dto)
        {
            try
            {
                var call = await _callService.StartCallAsync(dto);
                if (call == null) return BadRequest(ApiResponse<string>.FailResponse("Failed to start call"));
                return Ok(ApiResponse<CallDTO>.SuccessResponse(call, "Call started successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error starting call", new List<string> { ex.Message }));
            }
        }

        [HttpPost("end")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> EndCall([FromBody] EndCallDTO dto)
        {
            try
            {
                var success = await _callService.EndCallAsync(dto);
                if (!success) return BadRequest(ApiResponse<string>.FailResponse("Failed to end call"));
                return Ok(ApiResponse<string>.SuccessResponse("Call ended successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error ending call", new List<string> { ex.Message }));
            }
        }

        [HttpPost("transcript-segment")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddTranscriptSegment([FromBody] TranscriptSegmentDTO dto)
        {
            try
            {
                var success = await _callService.AddTranscriptSegmentAsync(dto);
                if (!success) return BadRequest(ApiResponse<string>.FailResponse("Failed to add transcript segment"));
                return Ok(ApiResponse<string>.SuccessResponse("Transcript segment added"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error adding transcript segment", new List<string> { ex.Message }));
            }
        }

        [HttpGet("{callId}")]
        [ProducesResponseType(typeof(ApiResponse<CallDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCall(Guid callId)
        {
            try
            {
                var call = await _callService.GetCallByIdAsync(callId);
                if (call == null) return NotFound(ApiResponse<string>.FailResponse("Call not found"));
                return Ok(ApiResponse<CallDTO>.SuccessResponse(call));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching call", new List<string> { ex.Message }));
            }
        }

        [HttpGet("client/{clientId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CallDTO>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCallsByClient(Guid clientId)
        {
            try
            {
                var calls = await _callService.GetCallsByClientIdAsync(clientId);
                return Ok(ApiResponse<IEnumerable<CallDTO>>.SuccessResponse(calls));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching calls", new List<string> { ex.Message }));
            }
        }

        [HttpPost("generate-followup")]
        [ProducesResponseType(typeof(ApiResponse<FollowUpMessageDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GenerateFollowUp([FromBody] GenerateFollowUpDTO dto)
        {
            try
            {
                var message = await _callService.GenerateFollowUpMessageAsync(dto);
                return Ok(ApiResponse<FollowUpMessageDTO>.SuccessResponse(message, "Follow-up generated"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error generating follow-up", new List<string> { ex.Message }));
            }
        }

        [HttpPost("approve-followup/{messageId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ApproveFollowUp(Guid messageId)
        {
            try
            {
                var success = await _callService.ApproveFollowUpMessageAsync(messageId);
                if (!success) return BadRequest(ApiResponse<string>.FailResponse("Failed to approve"));
                return Ok(ApiResponse<string>.SuccessResponse("Follow-up approved"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error approving follow-up", new List<string> { ex.Message }));
            }
        }

        [HttpPost("send-followup/{messageId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SendFollowUp(Guid messageId)
        {
            try
            {
                var success = await _callService.SendFollowUpMessageAsync(messageId);
                if (!success) return BadRequest(ApiResponse<string>.FailResponse("Failed to send"));
                return Ok(ApiResponse<string>.SuccessResponse("Follow-up sent"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error sending follow-up", new List<string> { ex.Message }));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("dashboard/metrics")]
        [ProducesResponseType(typeof(ApiResponse<DashboardMetricsDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDashboardMetrics()
        {
            try
            {
                var metrics = await _callService.GetDashboardMetricsAsync();
                return Ok(ApiResponse<DashboardMetricsDTO>.SuccessResponse(metrics));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching metrics", new List<string> { ex.Message }));
            }
        }

        [Authorize]
        [HttpGet("dashboard/client")]
        [ProducesResponseType(typeof(ApiResponse<ClientDashboardDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetClientDashboard()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    return Unauthorized(ApiResponse<string>.FailResponse("Invalid user"));
                var dashboard = await _callService.GetClientDashboardAsync(userId);
                return Ok(ApiResponse<ClientDashboardDTO>.SuccessResponse(dashboard));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching dashboard", new List<string> { ex.Message }));
            }
        }
    }
}