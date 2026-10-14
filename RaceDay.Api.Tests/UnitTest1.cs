using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace RaceDay.Api.Tests
{
    using global::RaceDay.Api.Models;
    using Microsoft.AspNetCore.Mvc.Testing;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;

    namespace RaceDay.Api.Tests
    {
       
        public class RaceDayApiTests : IClassFixture<WebApplicationFactory<Program>>
        {
            private readonly HttpClient _client;

            public RaceDayApiTests(WebApplicationFactory<Program> factory)
            {
                
                _client = factory.CreateClient();
            }

            [Fact]
            public async Task Get_Endpoints_ReturnSuccessAndCorrectContentType()
            {
                // Arrange - Replace with a real route from your API (e.g., "api/races" or "weatherforecast")
                var route = "api / races ";

                // Act - Send a GET request to the in-memory API
                var response = await _client.GetAsync(route);

                // Assert - Check that the response status code is 200 OK
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                // Check that it returns JSON data
                Assert.Equal("application/json; charset=utf-8",
                    response.Content.Headers.ContentType?.ToString());
            }
        }
    }

}
