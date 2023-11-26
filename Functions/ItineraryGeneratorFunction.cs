using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace az_function
{
    public class ItineraryGeneratorFunction
    {
        private readonly ILLMCompletionGenerator _llmCompletionGenerator;

        public ItineraryGeneratorFunction(ILLMCompletionGenerator llmCompletionGenerator)
        {
            _llmCompletionGenerator = llmCompletionGenerator;
        }

        [FunctionName("ItineraryGeneratorJob_GetItinerary")]
        public async Task<string> GenerateItinerary([ActivityTrigger] GetItineraryRequest getItineraryRequest, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _llmCompletionGenerator.GenerateCompletion(getItineraryRequest, log);

            return result;
        }
    }
}