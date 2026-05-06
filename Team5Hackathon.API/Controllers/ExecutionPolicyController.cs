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
    //[Authorize(Roles = "Admin")]
    public class ExecutionPolicyController : ControllerBase
    {
        private readonly IExecutionPolicyService _policyService;

        public ExecutionPolicyController(IExecutionPolicyService policyService)
        {
            _policyService = policyService;
        }

        [HttpGet("{policyId}")]
        [ProducesResponseType(typeof(ApiResponse<ExecutionPolicyDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPolicy(Guid policyId)
        {
            try
            {
                var policy = await _policyService.GetPolicyByIdAsync(policyId);
                if (policy == null) return NotFound(ApiResponse<string>.FailResponse("Policy not found"));
                return Ok(ApiResponse<ExecutionPolicyDTO>.SuccessResponse(policy));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching policy", new List<string> { ex.Message }));
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ExecutionPolicyDTO>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllPolicies()
        {
            try
            {
                var policies = await _policyService.GetAllPoliciesAsync();
                return Ok(ApiResponse<IEnumerable<ExecutionPolicyDTO>>.SuccessResponse(policies));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching policies", new List<string> { ex.Message }));
            }
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ExecutionPolicyDTO>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetActivePolicies()
        {
            try
            {
                var policies = await _policyService.GetActivePoliciesAsync();
                return Ok(ApiResponse<IEnumerable<ExecutionPolicyDTO>>.SuccessResponse(policies));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching active policies", new List<string> { ex.Message }));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ExecutionPolicyDTO>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreatePolicy([FromBody] CreateExecutionPolicyDTO dto)
        {
            try
            {
                var policy = await _policyService.CreatePolicyAsync(dto);
                return CreatedAtAction(nameof(GetPolicy), new { policyId = policy.Id }, ApiResponse<ExecutionPolicyDTO>.SuccessResponse(policy, "Policy created"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.FailResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error creating policy", new List<string> { ex.Message }));
            }
        }

        [HttpPut("{policyId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdatePolicy(Guid policyId, [FromBody] UpdateExecutionPolicyDTO dto)
        {
            try
            {
                var success = await _policyService.UpdatePolicyAsync(policyId, dto);
                if (!success) return NotFound(ApiResponse<string>.FailResponse("Policy not found"));
                return Ok(ApiResponse<string>.SuccessResponse("Policy updated"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.FailResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error updating policy", new List<string> { ex.Message }));
            }
        }

        [HttpDelete("{policyId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeletePolicy(Guid policyId)
        {
            try
            {
                var success = await _policyService.DeletePolicyAsync(policyId);
                if (!success) return NotFound(ApiResponse<string>.FailResponse("Policy not found"));
                return Ok(ApiResponse<string>.SuccessResponse("Policy deleted"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error deleting policy", new List<string> { ex.Message }));
            }
        }

        [HttpGet("evaluate")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> EvaluateExecutionMode([FromQuery] double riskScore, [FromQuery] string customerType)
        {
            try
            {
                var mode = await _policyService.EvaluateExecutionModeAsync(riskScore, customerType);
                return Ok(ApiResponse<string>.SuccessResponse(mode));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error evaluating execution mode", new List<string> { ex.Message }));
            }
        }
    }
}