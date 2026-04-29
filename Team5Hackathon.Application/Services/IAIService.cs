using System.Threading.Tasks;
using Team5Hackathon.Application.DTOs.AI;

namespace Team5Hackathon.Application.Services
{
    public interface IAIService
    {
        Task<string> ExtractIntentAsync(string text);
        Task<string> GenerateSummaryAsync(string transcript);
        Task<string> GenerateActionItemsAsync(string transcript);
        Task<string> GenerateFollowUpContentAsync(string intent, string summary);

        /// <summary>
        /// Analyses a customer complaint and returns a structured result containing
        /// summary, category, sentiment, a Nigerian-Pidgin response, and a formal
        /// English response.
        /// </summary>
        Task<ComplaintAnalysisResult> AnalyzeComplaintAsync(
            string transcribedText,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates a short Nigerian-Pidgin spoken reply for the complaint.
        /// </summary>
        Task<string> GeneratePidginResponseAsync(
            string transcribedText,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates a formal English reply for the complaint.
        /// </summary>
        Task<string> GenerateEnglishResponseAsync(
            string transcribedText,
            CancellationToken cancellationToken = default);
    }
}