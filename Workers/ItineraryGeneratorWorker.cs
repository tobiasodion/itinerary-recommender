using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace az_function
{
    public static class ItineraryGeneratorWorker
    {
        [FunctionName("ItineraryGeneratorWorker")]
        public static async Task Run(
                [QueueTrigger("getitineraryqueue")] GetItineraryRequest queueItem,
                [DurableClient] IDurableOrchestrationClient starter,
                ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync<GetItineraryRequest>("ItineraryGeneratorJob", null, queueItem);

            log.LogInformation($"Started orchestration with ID = '{instanceId}' for queue message: {queueItem}");
        }
    }
}