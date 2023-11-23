using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace az_function
{
    public static class StartChain
    {
        [FunctionName("StartChain")]
        public static async Task Run(
                [QueueTrigger("getitineraryqueue")] string queueItem,
                [DurableClient] IDurableOrchestrationClient starter,
                ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync<string>("Chaining", null, queueItem);

            log.LogInformation($"Started orchestration with ID = '{instanceId}' for queue message: {queueItem}");
        }
    }
}