using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using System.Net;

namespace az_function
{
    public static class GetItinerary
    {
        [FunctionName("GetItinerary")]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(RequestErrorModel), StatusCodes.Status400BadRequest)]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
        [RequestBodyType(typeof(GetItineraryRequest), "request")] HttpRequest req,
        [Queue("getitineraryqueue"), StorageAccount("AzureWebJobsStorage")] ICollector<GetItineraryRequest> msg,
        ILogger log)
        {
            log.LogInformation($"GetItinerary request received");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                GetItineraryRequest request = JsonConvert.DeserializeObject<GetItineraryRequest>(requestBody);
                msg.Add(request);
                return new AcceptedResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }

            var getItineraryRequestExample = new GetItineraryRequest("John", "Doe", "johndoe@gmail.com", "Paris");
            var errorMessage = $"Request body json must be - {getItineraryRequestExample}";
            var requestErrorModel = new RequestErrorModel(HttpStatusCode.BadRequest, errorMessage);

            return new BadRequestObjectResult(requestErrorModel);
        }
    }
}