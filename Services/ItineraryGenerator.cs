using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using Microsoft.WindowsAzure.Storage;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using Microsoft.WindowsAzure.Storage.Blob;

namespace az_function
{
    public static class ItineraryGenerator
    {
        [FunctionName("ItineraryGeneratorJob_GetItinerary")]
        public static async Task<string> GenerateItinerary([ActivityTrigger] GetItineraryRequest request, ILogger log)
        {
            // Replace "YOUR_OPENAI_API_KEY" with your actual OpenAI API key
            var apiKey = "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";
            var apiUrl = "https://api.openai.com/v1/completions";
            var prompt = $"Write a 8-line poem about {request.City}";
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