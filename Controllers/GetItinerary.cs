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
using System.Web.Http;

namespace az_function
{
    public static class GetItinerary
    {
        [FunctionName("GetItinerary")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
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
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error,
                };

                GetItineraryRequest request = JsonConvert.DeserializeObject<GetItineraryRequest>(requestBody, settings);
                log.LogInformation($"{request}");
                msg.Add(request);
                return new AcceptedResult();
            }
            catch (JsonSerializationException ex)
            {
                log.LogError(ex.Message);
                var getItineraryRequestExample = new GetItineraryRequest("John", "Doe", "johndoe@gmail.com", "Paris");
                var errorMessage = $"Request body json must be - {getItineraryRequestExample}";
                var badRequestResponse = new BadRequestResponse((int)HttpStatusCode.BadRequest, errorMessage);
                return new BadRequestObjectResult(badRequestResponse);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new InternalServerErrorResult();
            }
        }
    }
}