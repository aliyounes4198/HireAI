using System.Net.Http.Json;

namespace HireAI.API.Services
{
    public class OllamaService
    {
        private readonly HttpClient _httpClient;

        public OllamaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> AskAI(string prompt)
        {
            var request = new
            {
                model = "llama3.2:3b",
                prompt = prompt,
                stream = false,
                format = "json"
            };

            var response = await _httpClient.PostAsJsonAsync(
                "api/generate",
                request
            );

            response.EnsureSuccessStatusCode();

            var result =
                await response.Content.ReadFromJsonAsync<OllamaResponse>();

            return result?.Response ?? string.Empty;
        }

        private class OllamaResponse
        {
            public string Response { get; set; } = string.Empty;
        }
    }
}