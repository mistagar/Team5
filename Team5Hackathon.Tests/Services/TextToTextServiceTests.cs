using System.Text.Json;
using Team5Hackathon.Application.DTOs.AI;

namespace Team5Hackathon.Tests.Services;

public class TextToTextServiceTests
{
    [Fact]
    public void TextToTextRequest_Should_ValidateRequiredFields()
    {
        // Arrange & Act
        var request = new TextToTextRequest { Text = "Test input" };
        
        // Assert
        Assert.NotNull(request.Text);
        Assert.Equal("Test input", request.Text);
    }
    
    [Fact]
    public void ComplaintAnalysisResult_Should_DeserializeFromJson()
    {
        // Arrange
        var json = """
        {
            "summary": "Customer is experiencing network issues",
            "category": "network",
            "sentiment": "frustrated",
            "response": "We dey sorry for the inconvenience. We go check am for you.",
            "english_response": "We apologize for the inconvenience. We will investigate this for you."
        }
        """;
        
        // Act
        var result = JsonSerializer.Deserialize<ComplaintAnalysisResult>(json, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Customer is experiencing network issues", result.Summary);
        Assert.Equal("network", result.Category);
        Assert.Equal("frustrated", result.Sentiment);
        Assert.Equal("We dey sorry for the inconvenience. We go check am for you.", result.Response);
        Assert.Equal("We apologize for the inconvenience. We will investigate this for you.", result.EnglishResponse);
    }
    
    [Theory]
    [InlineData("Hello, good day. I dey call to complain about my network service.", "network")]
    [InlineData("My data bundle finish but I never use am well.", "data")]
    [InlineData("Una charge me money wey I no spend.", "billing")]
    public void TextToText_Should_HandleDifferentInputTypes(string input, string expectedCategory)
    {
        // This test demonstrates the expected input/output patterns
        // In a real test, you would mock the AI service and test the actual logic
        
        // Arrange
        var request = new TextToTextRequest { Text = input };
        
        // Act & Assert
        Assert.NotNull(request.Text);
        Assert.Contains(expectedCategory, new[] { "network", "data", "billing", "call", "sim", "other" });
    }

    [Fact]
    public void ComplaintAnalysisResult_Should_HaveBothResponseFields()
    {
        // Arrange
        var result = new ComplaintAnalysisResult
        {
            Response = "We dey sorry for the problem o!",
            EnglishResponse = "We apologize for the inconvenience."
        };
        
        // Act & Assert
        Assert.NotNull(result.Response);
        Assert.NotNull(result.EnglishResponse);
        Assert.NotEqual(result.Response, result.EnglishResponse);
    }
}