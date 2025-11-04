using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System;
namespace BeerCraftMVC.Services
{
    /// <summary>
    /// локален езиков модел Ollama 
    /// използва се Singleton Design Pattern, за да гарантира една инстанция на HttpClient - една точка за достъп до LLM услугата
    /// </summary>
    public sealed class LLMService
    {
        private static LLMService instance = null;
        private static readonly object padlock = new object();

        private readonly HttpClient _httpClient;
        private readonly string _ollamaUrl = "http://localhost:11434/api/generate";

   
        private LLMService()
        {
            _httpClient = new HttpClient();
        }


        public static LLMService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new LLMService();
                        }
                    }
                }
                return instance;
            }
        }
        public async Task<string> GetSuggestionAsync(string prompt, string modelName = "llama3")
        {
            string systemContext = "You are 'BeerCraft Assistant', a helpful AI assistant for a homebrewing application. " +
                           "Your entire purpose is to help users with beer brewing. " +
                           "All of your answers must be strictly related to beer, brewing recipes, ingredients, fermentation, or brewing techniques. " +
                           "If the user asks about something unrelated (like dessert), politely decline and steer the conversation back to brewing.";
            string finalPrompt = $"{systemContext}\n\nUSER PROMPT: {prompt}";
            try
            {
                var requestedData = new
                {
                    model = modelName,
                    prompt = finalPrompt,
                    stream = false
                };
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_ollamaUrl, requestedData);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
                    if (responseJson.TryGetProperty("response", out var responseText))
                    {
                        return responseText.GetString() ?? "No response";
                    }
                    return "Response parsed, but field not found.";
                }
                else
                {
                    return $"Error:{response.StatusCode}-{await response.Content.ReadAsStringAsync()}";
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";

            }
        }
    }

}
