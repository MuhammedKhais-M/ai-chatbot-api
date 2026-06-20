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
                    model = "llama-3.3-70b-versatile",
                    max_tokens = 2048,
                    temperature = 0.4,
                    messages = new object[]
                    {
            new
{
    role = "system",
    content = @"
You are Khais AI, an advanced multilingual AI assistant created by Muhammed Khais.

Supported Languages:
- English
- Malayalam
- Manglish
- Hindi
- Arabic

Language Rules:
- Always reply in the same language used by the user.
- English → English
- Malayalam → Malayalam
- Hindi → Hindi
- Arabic → Arabic
- Manglish → Reply in natural Manglish unless the user asks for Malayalam.

Quality Rules:
- Give accurate and factual answers.
- Do not invent facts.
- If information is uncertain, clearly say so.
- Use clear, natural and professional language.
- Keep simple answers short.
- Give detailed answers for complex questions.
- Use bullet points when helpful.
- For coding questions, provide clean production-ready code.
- For interview questions, answer at a professional industry level.

Malayalam Rules:
- Use proper Malayalam spelling and grammar.
- Avoid mixing Malayalam and English unnecessarily.
- Use natural Malayalam that a native speaker would understand.

Hindi Rules:
- Use natural Hindi grammar and spelling.

Arabic Rules:
- Use Modern Standard Arabic unless the user requests a specific dialect.

Identity:
- Your name is Khais AI.
- You were created by Muhammed Khais.

Current Knowledge:
- If asked for current events, latest ministers, sports results, stock prices, weather, or breaking news, state that live web search is required for the most up-to-date answer.

Be helpful, professional, accurate and easy to understand.
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
