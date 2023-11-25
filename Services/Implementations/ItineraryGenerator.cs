using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Configuration;

namespace az_function
{
    public class ItineraryGenerator : ILLMCompletionGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly IGptClient _gptClient;

        public ItineraryGenerator(IConfiguration configuration, IGptClient gptClient)
        {
            _configuration = configuration;
            _gptClient = gptClient;
        }

        public async Task<string> GenerateCompletion(GetItineraryRequest getItineraryRequest, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Replace "YOUR_OPENAI_API_KEY" with your actual OpenAI API key
            var apiKey = _configuration["GPT_API_KEY"];
            var apiUrl = "https://api.openai.com/v1/completions";
            var prompt = $"Write a 8-line poem about {getItineraryRequest.City}";
            var gptModel = "text-davinci-003"; // Specify the model you want to use
            var maxToken = 150;

            var getCompletionRequest = new GetCompletionRequest(apiKey, apiUrl, prompt, gptModel, maxToken);

            try
            {
                var itineraryFromGpt = (ObjectResult)await _gptClient.GetCompletion(getCompletionRequest, log);

                if (itineraryFromGpt.StatusCode != 200)
                {
                    log.LogError($"Something went wrong during gpt request");
                    throw new Exception($"Http Error {itineraryFromGpt.StatusCode} - Something went wrong during gpt request");
                }
                else
                {
                    var text = itineraryFromGpt.Value.ToString();
                    log.LogInformation(text);
                    return text;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}