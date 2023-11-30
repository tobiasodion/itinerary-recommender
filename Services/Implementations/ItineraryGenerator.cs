using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace az_function
{
    public class ItineraryGenerator : ILLMCompletionGenerator
    {
        private readonly IGptClient _gptClient;
        private readonly IConfiguration _configuration;

        public ItineraryGenerator(IGptClient gptClient, IConfiguration configuration)
        {
            _gptClient = gptClient;
            _configuration = configuration;
        }

        public async Task<string> GenerateCompletion(GetItineraryRequest getItineraryRequest, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var baseUrl = _configuration["GPT_API_BASE_URL"];
            var completionEndpoint = _configuration["GPT_API_COMPLETION_ENDPOINT"];

            var apiUrl = UrlHelper.GetFullUrl(baseUrl, completionEndpoint);
            log.LogInformation(apiUrl);
            var prompt = $"List itinerary for a day tour of {StringHelper.CapitalizeFirstLetter(getItineraryRequest.City)}. just a numbered list split into morning, afternoon, lunch, evening and nothing more";
            var gptModel = "text-davinci-003"; // Specify the model you want to use
            var maxToken = 600;

            var getCompletionRequest = new GetCompletionRequest(apiUrl, prompt, gptModel, maxToken);

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