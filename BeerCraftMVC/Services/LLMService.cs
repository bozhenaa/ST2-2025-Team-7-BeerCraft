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
    /// използва се Singleton Design Pattern, за да гарантира една инстанция на LLMService - една точка за достъп до LLM услугата в цялото приложение
    /// HttpClient се инстанцира веднъж и се преизползва
    /// </summary>
    public sealed class LLMService //не се наследява
    {
        private static LLMService instance = null; //единствената инстанция на класа
        private static readonly object padlock = new object(); //катинар за lock конструкцията

        private readonly HttpClient _httpClient; //клиентът за изпращане на заявки
        private readonly string _ollamaUrl = "http://localhost:11434/api/generate"; //адресът на ollama

        /// <summary>
        /// private конструктор, част от Singleton Design Pattern
        /// не може да бъде извикан извън класа, заради което използваме Instance
        /// </summary>
        private LLMService()
        {
            //единственото извикване на HttpClient
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Точка за достъп - публична, до единствената инстанция на LLMService
        /// От тук цялото приложение ще достъпва услугата
        /// </summary>
        public static LLMService Instance
        {
            get
            {
                //първа проверка, която проверява дали инстанцията е създадена
                if (instance == null)
                {
                    //гаранция, че само една нишка може да ползва този код
                    lock (padlock)
                    {
                        //отново се проверява, за да не е станало създаване на нова инстанция, докато нишките са изчаквали
                        if (instance == null)
                        {
                            //при преминаване на проверките е безопасно създаването на нова инстанция
                            instance = new LLMService();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// изпращане на промпт към Ollama - и връщане чрез текст
        /// </summary>
        /// <param name="prompt">Въпрос</param>
        /// <param name="modelName">Името на модела</param>
        /// <returns>Текстов отговор от езиковия модел/съобщение за грешка</returns>
        public async Task<string> GetSuggestionAsync(string prompt, string modelName = "llama3")
        {
            //Prompt Engineering -  дефинираме контекст, роля, правила
            string systemContext = "You are 'BeerCraft Assistant', a helpful AI assistant for a homebrewing application. " +
                           "Your entire purpose is to help users with beer brewing. " +
                           "All of your answers must be strictly related to beer, brewing recipes, ingredients, fermentation, or brewing techniques. " +
                           "If the user asks about something unrelated (like dessert), politely decline and steer the conversation back to brewing.";
            //комбинация на системния контекст и реалния потребителски промпт
            string finalPrompt = $"{systemContext}\n\nUSER PROMPT: {prompt}";
            try
            {
                //анонимен обект, който е JSON тялото, което Ollama очаква
                var requestedData = new
                {
                    model = modelName, //модела
                    prompt = finalPrompt, //пълния промпт
                    stream = false //пълния отговор се получава наведнъж
                };
                //POST заявка към URL-a на Ollama като JSON низ
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_ollamaUrl, requestedData);
               //проверява се HTTP статуса
                if (response.IsSuccessStatusCode)
                {
                    //JsonElement за четене от JSON структурата
                    var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
                    //достъпваме response
                    if (responseJson.TryGetProperty("response", out var responseText))
                    {
                        //ако го има го вземаме като низ
                        return responseText.GetString() ?? "No response";
                    }
                    //няма response
                    return "Response parsed, but field not found.";
                }
                //върната грешка, връщаме статус кода и съдържание
                else
                {
                    return $"Error:{response.StatusCode}-{await response.Content.ReadAsStringAsync()}";
                }
            }
            catch (Exception ex)
            {
                //улавяне на останалите по-прости грешки
                return $"Exception: {ex.Message}";

            }
        }
    }

}
