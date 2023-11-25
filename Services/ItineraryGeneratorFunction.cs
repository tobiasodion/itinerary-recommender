using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System;

namespace az_function
{
    public static class ItineraryGenerator
    {
        public static async Task<string> GenerateItinerary(GetItineraryRequest getItineraryRequest, ILogger log)
        {
            // Replace "YOUR_OPENAI_API_KEY" with your actual OpenAI API key
            var apiKey = "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";
            var apiUrl = "https://api.openai.com/v1/completions";
            var prompt = $"Write a 8-line poem about {getItineraryRequest.City}";
            var gptModel = "text-davinci-003"; // Specify the model you want to use
            var maxToken = 150;

            var getCompletionRequest = new GetCompletionRequest(apiKey, apiUrl, prompt, gptModel, maxToken);

            try
            {
                var itineraryFromGpt = (ObjectResult)await GptClient.GetCompletion(getCompletionRequest, log);

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