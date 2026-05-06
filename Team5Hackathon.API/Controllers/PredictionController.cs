using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Team5Hackathon.API.DTOs;
using Team5Hackathon.Application.Services;
using Team5Hackathon.Domain.Entities;

namespace Team5Hackathon.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredictionController : ControllerBase
    {
        private readonly IPredictionQueryService _predictionService;

        public PredictionController(IPredictionQueryService predictionService)
        {
            _predictionService = predictionService;
        }

        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerRiskProfile>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetByCustomerId(string customerId)
        {
            try
            {
                var profile = await _predictionService.GetByCustomerIdAsync(customerId);
                if (profile == null) return NotFound(ApiResponse<string>.FailResponse("Customer risk profile not found"));
                return Ok(ApiResponse<CustomerRiskProfile>.SuccessResponse(profile));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching customer risk profile", new List<string> { ex.Message }));
            }
        }

        [HttpGet("churn-score")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerRiskProfile>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetByChurnRiskScore([FromQuery] double minScore, [FromQuery] double maxScore)
        {
            try
            {
                var profiles = await _predictionService.GetByChurnRiskScoreRangeAsync(minScore, maxScore);
                return Ok(ApiResponse<IEnumerable<CustomerRiskProfile>>.SuccessResponse(profiles));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching profiles by churn risk score", new List<string> { ex.Message }));
            }
        }

        [HttpGet("customer-type/{customerType}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerRiskProfile>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetByCustomerType(string customerType)
        {
            try
            {
                var profiles = await _predictionService.GetByCustomerTypeAsync(customerType);
                return Ok(ApiResponse<IEnumerable<CustomerRiskProfile>>.SuccessResponse(profiles));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching profiles by customer type", new List<string> { ex.Message }));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerRiskProfile>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var profiles = await _predictionService.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<CustomerRiskProfile>>.SuccessResponse(profiles));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching all profiles", new List<string> { ex.Message }));
            }
        }
    }
}