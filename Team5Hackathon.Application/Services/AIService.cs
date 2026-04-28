using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Team5Hackathon.Application.Services
{
    public class AIService : IAIService
    {
        private readonly OpenAIClient _openAIClient;
        private readonly ILogger<AIService> _logger;
        private readonly string _deploymentName;

        public AIService(IConfiguration configuration, ILogger<AIService> logger)
        {
            var endpoint = configuration["AzureOpenAI:Endpoint"];
            var key = configuration["AzureOpenAI:Key"];
            _deploymentName = configuration["AzureOpenAI:DeploymentName"] ?? "gpt-35-turbo";
            _openAIClient = new OpenAIClient(new Uri(endpoint!), new AzureKeyCredential(key!));
            _logger = logger;
        }

        public async Task<string> ExtractIntentAsync(string text)
        {
            var options = new CompletionsOptions
            {
                DeploymentName = _deploymentName,
                Prompts = { $"Extract the primary intent from the following text: {text}. Possible intents: complaint, enquiry, other." },
                MaxTokens = 50
            };
            var response = await _openAIClient.GetCompletionsAsync(options);
            return response.Value.Choices[0].Text.Trim();
        }

        public async Task<string> GenerateSummaryAsync(string transcript)
        {
            var options = new CompletionsOptions
            {
                DeploymentName = _deploymentName,
                Prompts = { $"Summarize the following call transcript: {transcript}" },
                MaxTokens = 200
            };
            var response = await _openAIClient.GetCompletionsAsync(options);
            return response.Value.Choices[0].Text.Trim();
        }

        public async Task<string> GenerateActionItemsAsync(string transcript)
        {
            var options = new CompletionsOptions
            {
                DeploymentName = _deploymentName,
                Prompts = { $"Extract action items from the following call transcript: {transcript}" },
                MaxTokens = 200
            };
            var response = await _openAIClient.GetCompletionsAsync(options);
            return response.Value.Choices[0].Text.Trim();
        }

        public async Task<string> GenerateFollowUpContentAsync(string intent, string summary)
        {
            var options = new CompletionsOptions
            {
                DeploymentName = _deploymentName,
                Prompts = { $"Generate a follow-up message based on intent '{intent}' and summary '{summary}'." },
                MaxTokens = 200
            };
            var response = await _openAIClient.GetCompletionsAsync(options);
            return response.Value.Choices[0].Text.Trim();
        }
    }
}