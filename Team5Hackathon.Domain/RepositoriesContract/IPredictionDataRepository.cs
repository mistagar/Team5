using Team5Hackathon.Domain.Entities;

namespace Team5Hackathon.Domain.RepositoriesContract
{
    public interface IPredictionDataRepository
    {
        Task<(IEnumerable<Prediction> Predictions, int TotalCount)> GetAllPredictionsAsync(int page, int pageSize, string? customerType = null, double? minChurnRisk = null, double? maxChurnRisk = null, bool? churn = null);
        Task<Prediction?> GetPredictionByIdAsync(int id);
        Task<(IEnumerable<Prediction> Predictions, int TotalCount)> GetPredictionsByCustomerIdAsync(int customerId, int page = 1, int pageSize = 10);
        Task<IEnumerable<Prediction>> GetPredictionsByChurnRiskAsync(double minRisk, bool? churn = null);
    }
}