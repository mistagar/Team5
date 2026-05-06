using Microsoft.AspNetCore.Mvc;
using Team5Hackathon.Application.DTOs;
using Team5Hackathon.Application.Services;

namespace Team5Hackathon.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IPredictionDataService _predictionDataService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IPredictionDataService predictionDataService,
            ILogger<DashboardController> logger)
        {
            _predictionDataService = predictionDataService;
            _logger = logger;
        }

        /// <summary>
        /// Get all predictions from the predictions table with pagination and filtering
        /// </summary>
        /// <param name="page">Page number (default 1)</param>
        /// <param name="pageSize">Number of items per page (default 10)</param>
        /// <param name="customerType">Filter by customer type</param>
        /// <param name="minChurnRisk">Minimum churn risk score</param>
        /// <param name="maxChurnRisk">Maximum churn risk score</param>
        /// <param name="churn">Filter by churn status (true/false)</param>
        /// <returns>Paginated list of predictions</returns>
        [HttpGet("Get-All-Prediction")]
        public async Task<ActionResult<DashboardPredictionResponseDTO>> GetAllPredictions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? customerType = null,
            [FromQuery] double? minChurnRisk = null,
            [FromQuery] double? maxChurnRisk = null,
            [FromQuery] bool? churn = null)
        {
            try
            {
                var (predictions, totalCount) = await _predictionDataService.GetAllPredictionsAsync(page, pageSize, customerType, minChurnRisk, maxChurnRisk, churn);
                
                var predictionDTOs = predictions.Select(p => new PredictionDTO
                {
                    Id = p.Id,
                    CustomerId = p.CustomerId.GetValueOrDefault(),
                    AvgDataUsage = p.AvgDataUsage.GetValueOrDefault(),
                    UsageChangePct = p.UsageChangePct.GetValueOrDefault(),
                    RechargeFreq = p.RechargeFreq.GetValueOrDefault(),
                    RechargeChangePct = p.RechargeChangePct.GetValueOrDefault(),
                    EngagementScore = p.EngagementScore.GetValueOrDefault(),
                    NetworkQuality = p.NetworkQuality.GetValueOrDefault(),
                    HasComplaint = p.HasComplaint == true ? 1 : 0,
                    ComplaintText = p.ComplaintText,
                    Churn = p.Churn == true ? 1 : 0,
                    ChurnRiskScore = p.ChurnRiskScore.GetValueOrDefault(),
                    CustomerType = p.CustomerType ?? string.Empty,
                    Timestamp = p.Timestamp?.ToString() ?? string.Empty
                });

                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var response = new DashboardPredictionResponseDTO
                {
                    Predictions = predictionDTOs,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all predictions");
                return StatusCode(500, "Internal server error while retrieving predictions");
            }
        }

        /// <summary>
        /// Get prediction by ID
        /// </summary>
        /// <param name="id">Prediction ID</param>
        /// <returns>Prediction details</returns>
        [HttpGet("getPrediction/{id}")]
        public async Task<ActionResult<PredictionDTO>> GetPredictionById(int id)
        {
            try
            {
                var prediction = await _predictionDataService.GetPredictionByIdAsync(id);
                
                if (prediction == null)
                {
                    return NotFound($"Prediction with ID {id} not found");
                }

                var predictionDTO = new PredictionDTO
                {
                    Id = prediction.Id,
                    CustomerId = prediction.CustomerId.GetValueOrDefault(),
                    AvgDataUsage = prediction.AvgDataUsage.GetValueOrDefault(),
                    UsageChangePct = prediction.UsageChangePct.GetValueOrDefault(),
                    RechargeFreq = prediction.RechargeFreq.GetValueOrDefault(),
                    RechargeChangePct = prediction.RechargeChangePct.GetValueOrDefault(),
                    EngagementScore = prediction.EngagementScore.GetValueOrDefault(),
                    NetworkQuality = prediction.NetworkQuality.GetValueOrDefault(),
                    HasComplaint = prediction.HasComplaint == true ? 1 : 0,
                    ComplaintText = prediction.ComplaintText,
                    Churn = prediction.Churn == true ? 1 : 0,
                    ChurnRiskScore = prediction.ChurnRiskScore.GetValueOrDefault(),
                    CustomerType = prediction.CustomerType ?? string.Empty,
                    Timestamp = prediction.Timestamp?.ToString() ?? string.Empty
                };

                return Ok(predictionDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prediction with ID {Id}", id);
                return StatusCode(500, "Internal server error while retrieving prediction");
            }
        }

        /// <summary>
        /// Get predictions by customer ID with pagination
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="page">Page number (default 1)</param>
        /// <param name="pageSize">Number of items per page (default 10)</param>
        /// <returns>Paginated list of predictions for the customer</returns>
        [HttpGet("getPredictionsByCustomer/{customerId}")]
        public async Task<ActionResult<DashboardPredictionResponseDTO>> GetPredictionsByCustomerId(int customerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (predictions, totalCount) = await _predictionDataService.GetPredictionsByCustomerIdAsync(customerId, page, pageSize);
                
                var predictionDTOs = predictions.Select(p => new PredictionDTO
                {
                    Id = p.Id,
                    CustomerId = p.CustomerId.GetValueOrDefault(),
                    AvgDataUsage = p.AvgDataUsage.GetValueOrDefault(),
                    UsageChangePct = p.UsageChangePct.GetValueOrDefault(),
                    RechargeFreq = p.RechargeFreq.GetValueOrDefault(),
                    RechargeChangePct = p.RechargeChangePct.GetValueOrDefault(),
                    EngagementScore = p.EngagementScore.GetValueOrDefault(),
                    NetworkQuality = p.NetworkQuality.GetValueOrDefault(),
                    HasComplaint = p.HasComplaint == true ? 1 : 0,
                    ComplaintText = p.ComplaintText,
                    Churn = p.Churn == true ? 1 : 0,
                    ChurnRiskScore = p.ChurnRiskScore.GetValueOrDefault(),
                    CustomerType = p.CustomerType ?? string.Empty,
                    Timestamp = p.Timestamp?.ToString() ?? string.Empty
                });

                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var response = new DashboardPredictionResponseDTO
                {
                    Predictions = predictionDTOs,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving predictions for customer {CustomerId}", customerId);
                return StatusCode(500, "Internal server error while retrieving customer predictions");
            }
        }
    }
}