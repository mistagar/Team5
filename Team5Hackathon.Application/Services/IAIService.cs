using System.Threading.Tasks;

namespace Team5Hackathon.Application.Services
{
    public interface IAIService
    {
        Task<string> ExtractIntentAsync(string text);
        Task<string> GenerateSummaryAsync(string transcript);
        Task<string> GenerateActionItemsAsync(string transcript);
        Task<string> GenerateFollowUpContentAsync(string intent, string summary);
    }
}