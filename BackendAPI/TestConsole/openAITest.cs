using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    internal class openAITest
    {
        public void Go()
        {
            var client = new HttpClient();
            var apiKey = "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";
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var engine_id = "text-davinci-003";

            //text - davinci - 003
            //await GetEnginesAsync(apiKey);

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.openai.com/v1/engines/{engine_id}/completions");
            var input = new
            {
                prompt = "你好",
                max_tokens = 100,
                n = 1,
                stop = new string[] { "." },
                temperature = 0.5
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");

            var response = client.SendAsync(request).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// List all engines via the API
        /// </summary>
        /// <param name="auth">API authentication in order to call the API endpoint.  If not specified, attempts to use a default.</param>
        /// <returns>Asynchronously returns the list of all <see cref="Engine"/>s</returns>
        public static async Task GetEnginesAsync(string apiKey)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            client.DefaultRequestHeaders.Add("User-Agent", "williamwelsh/openai-dotnet");

            var response = await client.GetAsync("https://api.openai.com/v1/engines");
            var resultAsString = await response.Content.ReadAsStringAsync();
            //var resultAsString = await response.Content.ReadAsString();

            if (response.IsSuccessStatusCode)
            {
                //var engines = JsonConvert.DeserializeObject<JsonHelperRoot>(resultAsString).data;
                //return engines;
            }

            throw new HttpRequestException("Error calling OpenAi API to get list of engines.  HTTP status code: " + response.StatusCode + ". Content: " + resultAsString);
        }
    }
}
