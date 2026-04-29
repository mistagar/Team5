using System.Text.Json;
using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Team5Hackathon.Application.DTOs.AI;

namespace Team5Hackathon.Application.Services
{
    public class AIService : IAIService
    {
        private readonly OpenAIClient _openAIClient;
        private readonly ILogger<AIService> _logger;
        private readonly string _deploymentName;

        private const string AnalysisSystemPrompt =
            "You are a telecom customer experience assistant in Nigeria.\n\n" +
            "Tasks:\n" +
            "1. Convert the complaint into natural Nigerian Pidgin\n" +
            "2. Classify the issue into ONE category: network | data | billing | call | sim | other\n" +
            "3. Detect sentiment: positive | neutral | frustrated | angry\n" +
            "4. Respond in Nigerian Pidgin: apologize, show understanding, give solution, keep it short.\n" +
            "5. Respond in formal English: apologize professionally, show empathy, provide a clear solution.\n\n" +
            "IMPORTANT: Fix transcription errors. Do NOT change meaning. Preserve seriousness.\n\n" +
            "Return STRICT JSON (no markdown fences):\n" +
            "{\"summary\":\"...\",\"category\":\"...\",\"sentiment\":\"...\",\"response\":\"...\",\"english_response\":\"...\"}";

        private const string PidginReplySystemPrompt =
            "You are a Nigerian telecom customer support agent.\n" +
            "Detect the language/style of the complaint.\n" +
            "If Pidgin, respond in Pidgin. If English, respond in simple English.\n" +
            "Apologize politely. Provide a helpful solution. Keep it short.\n\n" +
            "Return STRICT JSON (no markdown fences):\n" +
            "{\"pidgin\":\"...\"}";

        private const string EnglishReplySystemPrompt =
            "You are a professional telecom customer support agent.\n" +
            "Read the customer complaint carefully.\n" +
            "Respond in clear, polite, formal English:\n" +
            "- Apologize sincerely\n" +
            "- Acknowledge the specific issue\n" +
            "- Provide a concrete solution or escalation path\n" +
            "- Keep the response concise and professional\n\n" +
            "Return STRICT JSON (no markdown fences):\n" +
            "{\"english\":\"...\"}";

        public AIService(IConfiguration configuration, ILogger<AIService> logger)
        {
            var endpoint = configuration["AzureOpenAI:Endpoint"];
            var key = configuration["AzureOpenAI:Key"];
            _deploymentName = configuration["AzureOpenAI:DeploymentName"] ?? "gpt-4o-mini";
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

        public async Task<ComplaintAnalysisResult> AnalyzeComplaintAsync(
            string transcribedText,
            CancellationToken cancellationToken = default)
        {
            var chatOptions = new ChatCompletionsOptions
            {
                DeploymentName = _deploymentName,
                Temperature = 0,
                Messages =
                {
                    new ChatRequestSystemMessage(AnalysisSystemPrompt),
                    new ChatRequestUserMessage(transcribedText)
                }
            };

            var response = await _openAIClient.GetChatCompletionsAsync(chatOptions, cancellationToken);
            var rawOutput = response.Value.Choices[0].Message.Content;

            _logger.LogDebug("AnalyzeComplaint raw output: {Output}", rawOutput);

            try
            {
                var result = JsonSerializer.Deserialize<ComplaintAnalysisResult>(rawOutput,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (result is not null)
                {
                    result.TranscribedText = transcribedText;
                    return result;
                }
                return new ComplaintAnalysisResult { TranscribedText = transcribedText, Summary = rawOutput };
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Could not parse complaint analysis JSON. Returning raw text as summary.");
                return new ComplaintAnalysisResult { TranscribedText = transcribedText, Summary = rawOutput };
            }
        }

        public async Task<string> GeneratePidginResponseAsync(
            string transcribedText,
            CancellationToken cancellationToken = default)
        {
            var chatOptions = new ChatCompletionsOptions
            {
                DeploymentName = _deploymentName,
                Temperature = 0.5f,
                Messages =
                {
                    new ChatRequestSystemMessage(PidginReplySystemPrompt),
                    new ChatRequestUserMessage(transcribedText)
                }
            };

            var response = await _openAIClient.GetChatCompletionsAsync(chatOptions, cancellationToken);
            var rawOutput = response.Value.Choices[0].Message.Content;

            _logger.LogDebug("GeneratePidginResponse raw output: {Output}", rawOutput);

            try
            {
                using var doc = JsonDocument.Parse(rawOutput);
                if (doc.RootElement.TryGetProperty("pidgin", out var pidginProp))
                    return pidginProp.GetString() ?? rawOutput;
            }
            catch (JsonException) { /* fall through */ }

            return rawOutput;
        }

        public async Task<string> GenerateEnglishResponseAsync(
            string transcribedText,
            CancellationToken cancellationToken = default)
        {
            var chatOptions = new ChatCompletionsOptions
            {
                DeploymentName = _deploymentName,
                Temperature = 0.5f,
                Messages =
                {
                    new ChatRequestSystemMessage(EnglishReplySystemPrompt),
                    new ChatRequestUserMessage(transcribedText)
                }
            };

            var response = await _openAIClient.GetChatCompletionsAsync(chatOptions, cancellationToken);
            var rawOutput = response.Value.Choices[0].Message.Content;

            _logger.LogDebug("GenerateEnglishResponse raw output: {Output}", rawOutput);

            try
            {
                using var doc = JsonDocument.Parse(rawOutput);
                if (doc.RootElement.TryGetProperty("english", out var englishProp))
                    return englishProp.GetString() ?? rawOutput;
            }
            catch (JsonException) { /* fall through */ }

            return rawOutput;
        }
    }
}