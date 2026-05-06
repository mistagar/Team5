using Microsoft.AspNetCore.Mvc;
using System.Net;
using Team5Hackathon.API.DTOs;
using Team5Hackathon.Application.DTOs;
using Team5Hackathon.Application.Services;

namespace Team5Hackathon.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DecisionController : ControllerBase
    {
        private readonly IDecisionEngineService _decisionService;

        public DecisionController(IDecisionEngineService decisionService)
        {
            _decisionService = decisionService;
        }

        [HttpGet("evaluate/{customerId}")]
        [ProducesResponseType(typeof(ApiResponse<DecisionEngineResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> EvaluateIntervention(string customerId)
        {
            try
            {
                var result = await _decisionService.EvaluateInterventionAsync(customerId);
                return Ok(ApiResponse<DecisionEngineResponse>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error evaluating intervention", new List<string> { ex.Message }));
            }
        }
    }
}