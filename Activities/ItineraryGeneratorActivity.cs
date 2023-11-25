using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace az_function
{
    public static class ItineraryGeneratorActivity
    {
        [FunctionName("ItineraryGeneratorJob_GetItinerary")]
        public static async Task<string> GenerateItinerary([ActivityTrigger] GetItineraryRequest getItineraryRequest, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var result = await ItineraryGenerator.GenerateItinerary(getItineraryRequest, log);

            return result;
        }
    }
}