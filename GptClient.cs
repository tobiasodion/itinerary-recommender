using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;

namespace az_function
{
    public static class GptClient
    {
        [FunctionName("Chaining_GetItineraryFromGpt")]
        public static async Task<IActionResult> GetItineraryFromGpt([ActivityTrigger] GetItineraryRequest request, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Replace "YOUR_OPENAI_API_KEY" with your actual OpenAI API key
            string apiKey = "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";
            string apiUrl = "https://api.openai.com/v1/completions";


            string prompt = $"Write a 8-line poem about {request.City}";

            // Set up the request payload for the OpenAI API
            var requestPayload = new
            {
                model = "text-davinci-003", // Specify the model you want to use
                prompt = prompt,
                max_tokens = 150
            };

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // Make a POST request to the OpenAI API
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, requestPayload);

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