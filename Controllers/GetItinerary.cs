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
    public static class GetItinerary
    {
        [FunctionName("GetItinerary")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        [Queue("getitineraryqueue"), StorageAccount("AzureWebJobsStorage")] ICollector<GetItineraryRequest> msg,
        ILogger log)
        {
            log.LogInformation($"GetItinerary request received");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            try
            {
                GetItineraryRequest request = JsonConvert.DeserializeObject<GetItineraryRequest>(requestBody);
                if (request != null)
                {
                    msg.Add(request);
                    return new AcceptedResult();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }

            return new BadRequestObjectResult("Please pass a request body - {\"FirstName\" : \"Tobias\", \"LastName\" : \"Odion\",\"Email\" : \"tobiasodion@gmail.com\",\"City\" : \"Paris\"}");
        }
    }
}