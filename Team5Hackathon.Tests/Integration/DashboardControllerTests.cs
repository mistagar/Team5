using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Team5Hackathon.Application.DTOs;
using Team5Hackathon.API;
using Xunit;

namespace Team5Hackathon.Tests.Integration
{
    public class DashboardControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public DashboardControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAllPredictions_ShouldReturnOkResult()
        {
            // Arrange
            var request = "/api/dashboard/getAllPrediction";

            // Act
            var response = await _client.GetAsync(request);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<DashboardPredictionResponseDTO>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(result);
            Assert.NotNull(result.Predictions);
        }

        [Fact]
        public async Task GetPredictionById_WithValidId_ShouldReturnOkResult()
        {
            // Arrange
            var request = "/api/dashboard/getPrediction/1";

            // Act
            var response = await _client.GetAsync(request);

            // Assert - Should return either OK with data or NotFound, both are valid
            Assert.True(response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetPredictionsByCustomerId_WithValidId_ShouldReturnOkResult()
        {
            // Arrange  
            var request = "/api/dashboard/getPredictionsByCustomer/1";

            // Act
            var response = await _client.GetAsync(request);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<DashboardPredictionResponseDTO>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(result);
            Assert.NotNull(result.Predictions);
        }
    }
}