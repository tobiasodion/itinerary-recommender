using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
                var blobUrl = await context.CallActivityAsync<string>("ItineraryGeneratorJob_UploadBlob", new PdfContent(pdfStreamFromString));
                await context.CallActivityAsync("ItineraryGeneratorJob_SendEMail", new SendEmailRequest(input.Email, $"1-Day {input.City} Tour Itinerary", $"Dear {input.FirstName},\n\nHere is your Itinerary for your {input.City} Trip.\n\nHave a great Trip!", pdfStreamFromString));
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
            }
        }
    }
}
