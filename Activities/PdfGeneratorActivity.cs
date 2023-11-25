using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace az_function
{
    public static class PdfGeneratorActivity
    {
        [FunctionName("ItineraryGeneratorJob_GeneratePdfStream")]
        public static async Task<byte[]> GeneratePdfStream([ActivityTrigger] string text, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var result = await PdfGenerator.GeneratePdfStream(text, log);

            return result;
        }
    }
}