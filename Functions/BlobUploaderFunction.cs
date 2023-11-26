using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace az_function
{
    public class BlobUploaderFunction
    {
        private readonly IFileUploader _blobUploader;

        public BlobUploaderFunction(IFileUploader blobUploader){
            _blobUploader = blobUploader;
        }


        [FunctionName("ItineraryGeneratorJob_UploadBlob")]
        public async Task<IActionResult> UploadBlob([ActivityTrigger] PdfContent pdfContent, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _blobUploader.UploadFile(pdfContent, log);

            return result;
        }
    }
}