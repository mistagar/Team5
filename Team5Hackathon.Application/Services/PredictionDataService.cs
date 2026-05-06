using Team5Hackathon.Domain.Entities;
using Team5Hackathon.Domain.RepositoriesContract;

namespace Team5Hackathon.Application.Services
{
    public class PredictionDataService : IPredictionDataService
    {
        private readonly IPredictionDataRepository _predictionRepository;

        public PredictionDataService(IPredictionDataRepository predictionRepository)
        {
            _predictionRepository = predictionRepository;
        }

        public async Task<(IEnumerable<Prediction> Predictions, int TotalCount)> GetAllPredictionsAsync(int page, int pageSize, string? customerType = null, double? minChurnRisk = null, double? maxChurnRisk = null, bool? churn = null)
        {
            return await _predictionRepository.GetAllPredictionsAsync(page, pageSize, customerType, minChurnRisk, maxChurnRisk, churn);
        }

        public async Task<Prediction?> GetPredictionByIdAsync(int id)
        {
            return await _predictionRepository.GetPredictionByIdAsync(id);
        }

        public async Task<(IEnumerable<Prediction> Predictions, int TotalCount)> GetPredictionsByCustomerIdAsync(int customerId, int page = 1, int pageSize = 10)
        {
            return await _predictionRepository.GetPredictionsByCustomerIdAsync(customerId, page, pageSize);
        }
    }
}