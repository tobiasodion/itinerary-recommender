using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace az_function
{
    public static class BlobUploaderActivity
    {
        [FunctionName("ItineraryGeneratorJob_UploadBlob")]
        public static async Task<IActionResult> UploadBlob([ActivityTrigger] PdfContent pdfContent, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var result = await BlobUploader.UploadBlob(pdfContent, log);

            return result;
        }
    }
}