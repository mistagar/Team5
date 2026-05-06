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
    public class SupervisorController : ControllerBase
    {
        private readonly ISupervisorApprovalService _approvalService;

        public SupervisorController(ISupervisorApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        [Authorize(Roles = "Supervisor,Admin")]
        [HttpGet("pending")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<InterventionQueueDTO>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPendingApprovals()
        {
            try
            {
                var pending = await _approvalService.GetPendingApprovalsAsync();
                return Ok(ApiResponse<IEnumerable<InterventionQueueDTO>>.SuccessResponse(pending));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching pending approvals", new List<string> { ex.Message }));
            }
        }

        [Authorize(Roles = "Supervisor,Admin")]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<InterventionQueueDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetApprovalById(Guid id)
        {
            try
            {
                var approval = await _approvalService.GetApprovalByIdAsync(id);
                if (approval == null) return NotFound(ApiResponse<string>.FailResponse("Approval not found"));
                return Ok(ApiResponse<InterventionQueueDTO>.SuccessResponse(approval));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching approval", new List<string> { ex.Message }));
            }
        }

        [Authorize(Roles = "Supervisor,Admin")]
        [HttpPost("{id}/approve")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ApproveIntervention(Guid id, [FromBody] ApproveInterventionDTO dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    return Unauthorized(ApiResponse<string>.FailResponse("Invalid user"));

                var success = await _approvalService.ApproveInterventionAsync(id, userId, dto.ModifiedOffer, dto.ModifiedChannel, dto.Notes);
                if (!success) return NotFound(ApiResponse<string>.FailResponse("Approval not found or not pending"));
                return Ok(ApiResponse<string>.SuccessResponse("Intervention approved"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error approving intervention", new List<string> { ex.Message }));
            }
        }

        [Authorize(Roles = "Supervisor,Admin")]
        [HttpPost("{id}/reject")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> RejectIntervention(Guid id, [FromBody] RejectInterventionDTO dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    return Unauthorized(ApiResponse<string>.FailResponse("Invalid user"));

                var success = await _approvalService.RejectInterventionAsync(id, userId, dto.Notes);
                if (!success) return NotFound(ApiResponse<string>.FailResponse("Approval not found or not pending"));
                return Ok(ApiResponse<string>.SuccessResponse("Intervention rejected"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error rejecting intervention", new List<string> { ex.Message }));
            }
        }

        [Authorize(Roles = "Supervisor,Admin")]
        [HttpPost("{id}/execute")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ExecuteIntervention(Guid id)
        {
            try
            {
                var success = await _approvalService.ExecuteInterventionAsync(id);
                if (!success) return NotFound(ApiResponse<string>.FailResponse("Intervention not found or not approved"));
                return Ok(ApiResponse<string>.SuccessResponse("Intervention executed"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error executing intervention", new List<string> { ex.Message }));
            }
        }

        [Authorize(Roles = "Supervisor,Admin")]
        [HttpGet("history")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<InterventionQueueDTO>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetApprovalHistory()
        {
            try
            {
                var history = await _approvalService.GetApprovalHistoryAsync();
                return Ok(ApiResponse<IEnumerable<InterventionQueueDTO>>.SuccessResponse(history));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching approval history", new List<string> { ex.Message }));
            }
        }

        [Authorize(Roles = "Supervisor,Admin")]
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<InterventionQueueDTO>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetByCustomerId(string customerId)
        {
            try
            {
                var queues = await _approvalService.GetByCustomerIdAsync(customerId);
                return Ok(ApiResponse<IEnumerable<InterventionQueueDTO>>.SuccessResponse(queues));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching interventions by customer", new List<string> { ex.Message }));
            }
        }
    }
}