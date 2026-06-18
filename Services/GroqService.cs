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

            if (string.IsNullOrEmpty(apiKey))
            {
                return "❌ Groq API key is missing in environment variables.";
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var requestBody = new
                {
                    model = "llama-3.1-8b-instant",
                    messages = new object[]
                    {
               new
  {
    role = "system",
    content = @"You are Khais AI, an AI assistant created by Muhammed Khais.

You understand:
- English
- Malayalam
- Manglish (Malayalam written using English letters)

Rules:
- If the user writes in Malayalam, reply in Malayalam.
- If the user writes in Manglish, reply in Malayalam.
- If the user writes in English, reply in English.
- Detect the language automatically.
- Be professional, helpful, and accurate.
"";

Rules:
- If the user writes in Malayalam, reply in Malayalam.
- If the user writes in English, reply in English.
- Detect the language automatically.
- Give accurate, helpful and professional answers.
- If the user asks current affairs, use the latest information available.
"
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
                    new StringContent(json, Encoding.UTF8, "application/json"));

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return $"❌ GROQ ERROR: {response.StatusCode} - {error}";
                }

                var responseJson = JObject.Parse(result);

                var aiResponse =
                    responseJson["choices"]?[0]?["message"]?["content"]?.ToString();

                return aiResponse ?? "No response received.";
            }
            catch (Exception ex)
            {
                return $"❌ Server Error: {ex.Message}";
            }
        }
    }
}
