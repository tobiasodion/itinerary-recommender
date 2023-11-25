using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;

namespace az_function
{
    public static class GptClient
    {
        public static async Task<IActionResult> GetCompletion(GetCompletionRequest getCompletionRequest, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Set up the request payload for the OpenAI API
            var requestPayload = new
            {
                model = getCompletionRequest.Model,
                prompt = getCompletionRequest.Prompt,
                max_tokens = getCompletionRequest.MaxToken
            };

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {getCompletionRequest.ApiKey}");

                // Make a POST request to the OpenAI API
                HttpResponseMessage response = await client.PostAsJsonAsync(getCompletionRequest.ApiUrl, requestPayload);

                if (response.IsSuccessStatusCode)
                {
                    // Read and return the response from the OpenAI API
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    dynamic test = JsonConvert.DeserializeObject(apiResponse);

                    // Extract the text using dynamic
                    string extractedText = test?.choices?[0]?.text;
                    return new OkObjectResult(extractedText);
                }
                else
                {
                    // Log the error if the API request is not successful
                    log.LogError($"Error calling OpenAI API: {response.StatusCode} - {response.ReasonPhrase}");
                    return new StatusCodeResult((int)response.StatusCode);
                }
            }
        }
    }
}