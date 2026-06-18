using System.Text;
using System.Text.Json;

namespace AIChatBot.API.Services
{
    public class TavilyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TavilyService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> SearchAsync(string query)
        {
            var apiKey = _configuration["Tavily:ApiKey"];

            var requestBody = new
            {
                api_key = apiKey,
                query = query,
                search_depth = "basic"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "https://api.tavily.com/search",
                content);

            return await response.Content.ReadAsStringAsync();
        }
    }
}