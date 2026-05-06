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
    public class OfferController : ControllerBase
    {
        private readonly IOfferManagementService _offerService;

        public OfferController(IOfferManagementService offerService)
        {
            _offerService = offerService;
        }

        [HttpGet("{offerId}")]
        [ProducesResponseType(typeof(ApiResponse<OfferConfigurationDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOffer(Guid offerId)
        {
            try
            {
                var offer = await _offerService.GetOfferByIdAsync(offerId);
                if (offer == null) return NotFound(ApiResponse<string>.FailResponse("Offer not found"));
                return Ok(ApiResponse<OfferConfigurationDTO>.SuccessResponse(offer));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching offer", new List<string> { ex.Message }));
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OfferConfigurationDTO>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllOffers()
        {
            try
            {
                var offers = await _offerService.GetAllOffersAsync();
                return Ok(ApiResponse<IEnumerable<OfferConfigurationDTO>>.SuccessResponse(offers));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching offers", new List<string> { ex.Message }));
            }
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OfferConfigurationDTO>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetActiveOffers()
        {
            try
            {
                var offers = await _offerService.GetActiveOffersAsync();
                return Ok(ApiResponse<IEnumerable<OfferConfigurationDTO>>.SuccessResponse(offers));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching active offers", new List<string> { ex.Message }));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<OfferConfigurationDTO>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateOffer([FromBody] CreateOfferDTO dto)
        {
            try
            {
                var offer = await _offerService.CreateOfferAsync(dto);
                return CreatedAtAction(nameof(GetOffer), new { offerId = offer.Id }, ApiResponse<OfferConfigurationDTO>.SuccessResponse(offer, "Offer created"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.FailResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error creating offer", new List<string> { ex.Message }));
            }
        }

        [HttpPut("{offerId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateOffer(Guid offerId, [FromBody] UpdateOfferDTO dto)
        {
            try
            {
                var success = await _offerService.UpdateOfferAsync(offerId, dto);
                if (!success) return NotFound(ApiResponse<string>.FailResponse("Offer not found"));
                return Ok(ApiResponse<string>.SuccessResponse("Offer updated"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.FailResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error updating offer", new List<string> { ex.Message }));
            }
        }

        [HttpDelete("{offerId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeactivateOffer(Guid offerId)
        {
            try
            {
                var success = await _offerService.DeactivateOfferAsync(offerId);
                if (!success) return NotFound(ApiResponse<string>.FailResponse("Offer not found"));
                return Ok(ApiResponse<string>.SuccessResponse("Offer deactivated"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error deactivating offer", new List<string> { ex.Message }));
            }
        }

        [HttpPost("{offerId}/rules")]
        [ProducesResponseType(typeof(ApiResponse<OfferEligibilityRuleDTO>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> AddEligibilityRule(Guid offerId, [FromBody] CreateEligibilityRuleDTO dto)
        {
            try
            {
                var rule = await _offerService.AddEligibilityRuleAsync(offerId, dto);
                return Created("", ApiResponse<OfferEligibilityRuleDTO>.SuccessResponse(rule, "Eligibility rule added"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.FailResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error adding eligibility rule", new List<string> { ex.Message }));
            }
        }

        [HttpPut("rules/{ruleId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateEligibilityRule(Guid ruleId, [FromBody] UpdateEligibilityRuleDTO dto)
        {
            try
            {
                var success = await _offerService.UpdateEligibilityRuleAsync(ruleId, dto);
                if (!success) return NotFound(ApiResponse<string>.FailResponse("Rule not found"));
                return Ok(ApiResponse<string>.SuccessResponse("Eligibility rule updated"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error updating eligibility rule", new List<string> { ex.Message }));
            }
        }

        [HttpDelete("rules/{ruleId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteEligibilityRule(Guid ruleId)
        {
            try
            {
                var success = await _offerService.DeleteEligibilityRuleAsync(ruleId);
                if (!success) return NotFound(ApiResponse<string>.FailResponse("Rule not found"));
                return Ok(ApiResponse<string>.SuccessResponse("Eligibility rule deleted"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error deleting eligibility rule", new List<string> { ex.Message }));
            }
        }

        [HttpGet("{offerId}/rules")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OfferEligibilityRuleDTO>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetEligibilityRules(Guid offerId)
        {
            try
            {
                var rules = await _offerService.GetEligibilityRulesByOfferIdAsync(offerId);
                return Ok(ApiResponse<IEnumerable<OfferEligibilityRuleDTO>>.SuccessResponse(rules));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Error fetching eligibility rules", new List<string> { ex.Message }));
            }
        }
    }
}