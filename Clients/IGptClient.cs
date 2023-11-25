using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace az_function
{
    public interface IGptClient
    {
        Task<IActionResult> GetCompletion(GetCompletionRequest getCompletionRequest, ILogger log);
    }
}