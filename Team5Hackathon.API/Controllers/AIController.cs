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

    public AIController(
        IAIService aiService,
        ITranscriptionService transcriptionService,
        ITextToSpeechService ttsService)
    {
        _aiService = aiService;
        _transcriptionService = transcriptionService;
        _ttsService = ttsService;
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
    public async Task<IActionResult> ProcessComplaint(
        IFormFile audioFile,
        CancellationToken cancellationToken)
    {
        if (audioFile is null || audioFile.Length == 0)
            return BadRequest(ApiResponse<string>.FailResponse("An audio file is required."));

        // 1. Read audio bytes
        await using var ms = new MemoryStream();
        await audioFile.CopyToAsync(ms, cancellationToken);
        var audioBytes = ms.ToArray();

        // 2. Transcribe
        var transcription = await _transcriptionService.TranscribeAsync(
            audioBytes,
            audioFile.FileName,
            audioFile.ContentType,
            cancellationToken);

        if (string.IsNullOrWhiteSpace(transcription))
            return BadRequest(ApiResponse<string>.FailResponse("Transcription failed or returned empty text."));

        // 3. Analyse complaint (returns pidgin + english responses in one call)
        var analysis = await _aiService.AnalyzeComplaintAsync(transcription, cancellationToken);

        // 4. Synthesise Pidgin response (Nigerian voice)
        var pidginText = string.IsNullOrWhiteSpace(analysis.Response) ? transcription : analysis.Response;
        var pidginAudio = await _ttsService.SynthesiseAsync(
            new TextToSpeechRequest { Text = pidginText, VoiceName = "en-NG-EzinneNeural" },
            cancellationToken);

        // 5. Synthesise English response (neutral English voice)
        var englishText = string.IsNullOrWhiteSpace(analysis.EnglishResponse) ? transcription : analysis.EnglishResponse;
        var englishAudio = await _ttsService.SynthesiseAsync(
            new TextToSpeechRequest { Text = englishText, VoiceName = "en-US-AriaNeural" },
            cancellationToken);

        var pipelineResponse = new ComplaintPipelineResponse
        {
            TranscribedText = transcription,
            Analysis = analysis,
            AudioResponseBase64 = pidginAudio.AudioBase64,
            EnglishAudioResponseBase64 = englishAudio.AudioBase64
        };

        return Ok(ApiResponse<ComplaintPipelineResponse>.SuccessResponse(
            pipelineResponse,
            "Complaint processed successfully."));
    }
}
