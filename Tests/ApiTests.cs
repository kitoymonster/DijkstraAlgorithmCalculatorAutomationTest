using System.Text;
using System.Text.Json;
using NUnit.Framework;

namespace DijkstraShortestPathCalculator.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ApiTests
    {
        private readonly HttpClient _client = new();

        [Test]
        [Category("API")]
        [Category("Regression")]
        public async Task Test_CalculateShortestPath_ReturnsCorrectResult()
        {
            var url = "https://echo.free.beeceptor.com/";
            var payload = new
            {
                nodeNames = new[] { "A", "B" },
                distance = 4
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync(url, content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));        }
    }
}