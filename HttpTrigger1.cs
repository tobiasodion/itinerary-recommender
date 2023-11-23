using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace az_function
{
    public static class HttpTrigger1
    {
        [FunctionName("HttpTrigger1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            GetItineraryRequest request = JsonConvert.DeserializeObject<GetItineraryRequest>(requestBody);
            var firstName = request.FirstName;

            string responseMessage = string.IsNullOrEmpty(firstName)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {firstName}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
