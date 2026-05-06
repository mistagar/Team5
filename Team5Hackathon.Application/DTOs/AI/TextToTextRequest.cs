using System.ComponentModel.DataAnnotations;

namespace Team5Hackathon.Application.DTOs.AI;

public sealed class TextToTextRequest
{
    [Required(ErrorMessage = "Input text is required.")]
    public required string Text { get; init; }
}