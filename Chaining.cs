using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace az_function
{
    public static class Chaining
    {
        [FunctionName("Chaining")]
        public static async Task RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context,
        ILogger log)
        {
            try
            {
                var input = context.GetInput<string>();
                log.LogInformation($"Request from context - {input}");
                var itineraryRequest = JsonConvert.DeserializeObject<GetItineraryRequest>(input);

                var itineraryFromGpt = await context.CallActivityAsync<ObjectResult>("Chaining_GetItineraryFromGpt", itineraryRequest);

                if (itineraryFromGpt.StatusCode != 200)
                {
                    log.LogError($"Something went wrong during gpt recommendation");
                }
                else
                {
                    var poem = itineraryFromGpt.Value.ToString();
                    log.LogInformation(poem);
                    var pdfFromString = await context.CallActivityAsync<byte[]>("Chaining_GeneratePdf", poem);
                    log.LogInformation(pdfFromString.ToString());
                    var blobUrl = await context.CallActivityAsync<ObjectResult>("Chaining_BlobUploader", new PdfContent{Content = pdfFromString});
                    if (blobUrl.StatusCode != 200)
                    {
                        log.LogError($"Something went wrong during blob upload");
                    }
                    else
                    {
                        log.LogInformation(blobUrl.Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
            }
        }
    }
}
