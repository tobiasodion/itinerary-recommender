using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace az_function
{
    public static class ItineraryGeneratorJob
    {
        [FunctionName("ItineraryGeneratorJob")]
        public static async Task RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context,
        ILogger log)
        {
            try
            {
                var input = context.GetInput<GetItineraryRequest>();
                var itineraryFromGpt = await context.CallActivityAsync<string>("ItineraryGeneratorJob_GetItinerary", input);
                var pdfStreamFromString = await context.CallActivityAsync<byte[]>("ItineraryGeneratorJob_GeneratePdfStream", itineraryFromGpt);
                log.LogInformation(pdfStreamFromString.ToString());
                var blobUrl = await context.CallActivityAsync<ObjectResult>("ItineraryGeneratorJob_UploadBlob", new PdfContent { Content = pdfStreamFromString });

                if (blobUrl.StatusCode != 200)
                {
                    log.LogError($"Something went wrong during blob upload");
                }
                else
                {
                    log.LogInformation(blobUrl.Value.ToString());
                }

            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
            }
        }
    }
}
