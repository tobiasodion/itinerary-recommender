using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace az_function
{
    public interface ILLMCompletionGenerator
    {
        Task<string> GenerateCompletion(GetItineraryRequest getItineraryRequest, ILogger log);
    }
}