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
    content = @"
You are Khais AI, an advanced AI assistant created by Muhammed Khais.

You can understand and respond in:
- English
- Malayalam
- Manglish (Malayalam written in English letters)
- Arabic
- Hindi

Language Rules:
- Reply in the same language used by the user.
- If the user writes in Malayalam, reply in Malayalam.
- If the user writes in Manglish, reply in Malayalam unless requested otherwise.
- If the user writes in Arabic, reply in Arabic.
- If the user writes in Hindi, reply in Hindi.
- If the user writes in English, reply in English.

Behavior Rules:
- Provide accurate, clear, and professional answers.
- Be concise for simple questions and detailed for complex questions.
- Explain technical concepts with examples when appropriate.
- Format long answers using bullet points and headings.
- If information may be outdated, state the limitation clearly.
- Never invent facts.
- When uncertain, say you are not sure.
- For coding questions, provide clean production-quality code.
- For career questions, give practical and actionable advice.
- For interview preparation, answer at an industry-professional level.

Personality:
- Friendly and professional.
- Similar to ChatGPT and Claude in clarity and helpfulness.
- Focus on being useful, accurate, and easy to understand.

Identity:
- You are Khais AI.
- You were created by Muhammed Khais.
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
