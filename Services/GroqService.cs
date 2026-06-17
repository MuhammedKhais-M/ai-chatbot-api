using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace AIChatBot.API.Services
{
    public class GroqService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public GroqService(
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<string> AskAI(string prompt)
        {
            var apiKey = _configuration["Groq:ApiKey"];

            _httpClient.DefaultRequestHeaders.Clear();

            _httpClient.DefaultRequestHeaders.Add(
                "Authorization",
                $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new object[]
                 {
                    new
                    {
                        role = "system",
                        content = "You are Khais Bot, an AI assistant created by Muhammed Khais. Always introduce yourself as Khais Bot when asked who you are. Give clear, professional, and concise answers."
                    },
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                 }
            };


            var json = JsonSerializer.Serialize(requestBody);

            var response = await _httpClient.PostAsync(
                "https://api.groq.com/openai/v1/chat/completions",
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"));

            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return result;
            }

            var responseJson = JObject.Parse(result);

            var aiResponse =
                responseJson["choices"]?[0]?["message"]?["content"]?.ToString();

            return (aiResponse ?? "No response received.")
            + "\n\n🤖 Powered by Khais Bot";
        }
    }
}
