using Microsoft.AspNetCore.Mvc;
using Team5Hackathon.API.DTOs;
using Team5Hackathon.Application.DTOs.AI;
using Team5Hackathon.Application.Services;

namespace Team5Hackathon.API.Controllers;

/// <summary>
/// Provides AI-powered complaint analysis, Pidgin response generation,
/// text-to-speech synthesis, and a full end-to-end complaint pipeline.
/// </summary>
[ApiController]
[Route("api/ai")]
public sealed class AIController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly ITranscriptionService _transcriptionService;
    private readonly ITextToSpeechService _ttsService;
    private readonly ILogger<AIController> _logger;

    public AIController(
        IAIService aiService,
        ITranscriptionService transcriptionService,
        ITextToSpeechService ttsService,
        ILogger<AIController> logger)
    {
        _aiService = aiService;
        _transcriptionService = transcriptionService;
        _ttsService = ttsService;
        _logger = logger;
    }

    // ??????????????????????????????????????????????????????????????????????????
    // POST api/ai/analyze
    // ??????????????????????????????????????????????????????????????????????????
    /// <summary>
    /// Analyses a customer complaint text and returns a structured result
    /// (summary, category, sentiment, Nigerian-Pidgin response).
    /// </summary>
    [HttpPost("analyze")]
    [ProducesResponseType(typeof(ApiResponse<ComplaintAnalysisResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AnalyzeComplaint(
        [FromBody] ComplaintAnalysisRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TranscribedText))
            return BadRequest(ApiResponse<string>.FailResponse("TranscribedText is required."));

        var result = await _aiService.AnalyzeComplaintAsync(request.TranscribedText, cancellationToken);
        return Ok(ApiResponse<ComplaintAnalysisResult>.SuccessResponse(result, "Complaint analysed successfully."));
    }

    // ??????????????????????????????????????????????????????????????????????????
    // POST api/ai/pidgin-response
    // ??????????????????????????????????????????????????????????????????????????
    /// <summary>
    /// Generates a short Nigerian-Pidgin reply for a customer complaint.
    /// </summary>
    [HttpPost("pidgin-response")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GeneratePidginResponse(
        [FromBody] ComplaintAnalysisRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TranscribedText))
            return BadRequest(ApiResponse<string>.FailResponse("TranscribedText is required."));

        var pidgin = await _aiService.GeneratePidginResponseAsync(request.TranscribedText, cancellationToken);
        return Ok(ApiResponse<string>.SuccessResponse(pidgin, "Pidgin response generated."));
    }

    // ??????????????????????????????????????????????????????????????????????????
    // POST api/ai/english-response
    // ??????????????????????????????????????????????????????????????????????????
    /// <summary>
    /// Generates a formal English reply for a customer complaint.
    /// </summary>
    [HttpPost("english-response")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateEnglishResponse(
        [FromBody] ComplaintAnalysisRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TranscribedText))
            return BadRequest(ApiResponse<string>.FailResponse("TranscribedText is required."));

        var english = await _aiService.GenerateEnglishResponseAsync(request.TranscribedText, cancellationToken);
        return Ok(ApiResponse<string>.SuccessResponse(english, "English response generated."));
    }

    // ??????????????????????????????????????????????????????????????????????????
    // POST api/ai/text-to-text
    // ??????????????????????????????????????????????????????????????????????????
    /// <summary>
    /// Direct text-to-text processing endpoint that analyzes input text and returns
    /// structured JSON with summary, category, sentiment, and response.
    /// Similar to the Jupyter notebook implementation.
    /// </summary>
    [HttpPost("text-to-text")]
    [ProducesResponseType(typeof(ApiResponse<ComplaintAnalysisResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessTextToText(
        [FromBody] TextToTextRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(ApiResponse<string>.FailResponse("Text is required."));

        var result = await _aiService.ProcessTextToTextAsync(request.Text, cancellationToken);
        return Ok(ApiResponse<ComplaintAnalysisResult>.SuccessResponse(result, "Text processed successfully."));
    }

    // ??????????????????????????????????????????????????????????????????????????
    // POST api/ai/process-complaint   (audio file upload)
    // ??????????????????????????????????????????????????????????????????????????
    /// <summary>
    /// Full end-to-end pipeline:
    /// 1. Transcribe the uploaded audio file (Whisper).
    /// 2. Analyse the complaint (GPT-4o-mini) — returns Pidgin + English responses.
    /// 3. Synthesise both Pidgin and English responses to MP3 (Azure TTS).
    /// Returns transcription, analysis, and base-64 audio for both languages.
    /// </summary>
    [HttpPost("process-complaint")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<ComplaintPipelineResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ProcessComplaint(
        IFormFile audioFile,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("ProcessComplaint endpoint called with file: {FileName}", audioFile?.FileName ?? "null");

            if (audioFile is null || audioFile.Length == 0)
                return BadRequest(ApiResponse<string>.FailResponse("An audio file is required."));

            // 1. Read audio bytes
            _logger.LogDebug("Reading audio file bytes. Size: {Size} bytes", audioFile.Length);
            await using var ms = new MemoryStream();
            await audioFile.CopyToAsync(ms, cancellationToken);
            var audioBytes = ms.ToArray();

            // 2. Transcribe
            _logger.LogDebug("Starting transcription with file: {FileName}, ContentType: {ContentType}", 
                audioFile.FileName, audioFile.ContentType);
                
            string transcription;
            try
            {
                transcription = await _transcriptionService.TranscribeAsync(
                    audioBytes,
                    audioFile.FileName,
                    audioFile.ContentType,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transcription service failed");
                return StatusCode(500, ApiResponse<string>.FailResponse($"Transcription failed: {ex.Message}"));
            }

            if (string.IsNullOrWhiteSpace(transcription))
            {
                _logger.LogWarning("Transcription returned empty or null result");
                return BadRequest(ApiResponse<string>.FailResponse("Transcription failed or returned empty text."));
            }

            _logger.LogDebug("Transcription completed. Length: {Length}", transcription.Length);

            // 3. Analyse complaint (returns pidgin + english responses in one call)
            ComplaintAnalysisResult analysis;
            try
            {
                analysis = await _aiService.AnalyzeComplaintAsync(transcription, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI analysis service failed");
                return StatusCode(500, ApiResponse<string>.FailResponse($"AI analysis failed: {ex.Message}"));
            }

            _logger.LogDebug("AI analysis completed");

            // 4. Synthesise Pidgin response (Nigerian voice)
            var pidginText = string.IsNullOrWhiteSpace(analysis.Response) ? transcription : analysis.Response;
            TextToSpeechResponse pidginAudio;
            
            try
            {
                pidginAudio = await _ttsService.SynthesiseAsync(
                    new TextToSpeechRequest { Text = pidginText, VoiceName = "en-NG-EzinneNeural" },
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TTS service failed for Pidgin audio");
                return StatusCode(500, ApiResponse<string>.FailResponse($"Pidgin TTS failed: {ex.Message}"));
            }

            _logger.LogDebug("Pidgin TTS completed");

            // 5. Synthesise English response (neutral English voice)
            var englishText = string.IsNullOrWhiteSpace(analysis.EnglishResponse) ? transcription : analysis.EnglishResponse;
            TextToSpeechResponse englishAudio;
            
            try
            {
                englishAudio = await _ttsService.SynthesiseAsync(
                    new TextToSpeechRequest { Text = englishText, VoiceName = "en-US-AriaNeural" },
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TTS service failed for English audio");
                return StatusCode(500, ApiResponse<string>.FailResponse($"English TTS failed: {ex.Message}"));
            }

            _logger.LogDebug("English TTS completed");

            var pipelineResponse = new ComplaintPipelineResponse
            {
                TranscribedText = transcription,
                Analysis = analysis,
                AudioResponseBase64 = pidginAudio.AudioBase64,
                EnglishAudioResponseBase64 = englishAudio.AudioBase64
            };

            _logger.LogInformation("ProcessComplaint pipeline completed successfully");

            return Ok(ApiResponse<ComplaintPipelineResponse>.SuccessResponse(
                pipelineResponse,
                "Complaint processed successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in ProcessComplaint endpoint");
            return StatusCode(500, ApiResponse<string>.FailResponse($"An unexpected error occurred: {ex.Message}"));
        }
    }
}
