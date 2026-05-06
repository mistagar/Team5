using System.Threading.Tasks;
using Team5Hackathon.Application.DTOs;

namespace Team5Hackathon.Application.Services
{
    public interface IDecisionEngineService
    {
        Task<DecisionEngineResponse> EvaluateInterventionAsync(string customerId);
    }
}