using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using Microsoft.WindowsAzure.Storage;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using Microsoft.WindowsAzure.Storage.Blob;

namespace az_function
{
    public static class BlobUploader
    {
        [FunctionName("Chaining_BlobUploader")]
        public static async Task<IActionResult> UploadBlob([ActivityTrigger] PdfContent pdfContent, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Upload the PDF to Azure Blob Storage
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=tobiasodionia;AccountKey=gRND5kdP37A19/heWLiSAWukLY7drgc3U/vYz6NWlL5g18DCZDjJoqg4BwGOmeaUHeTQV3p7nvoj+ASt0t/DYA==;EndpointSuffix=core.windows.net";
            string containerName = "ia-pdf";
            string blobName = Guid.NewGuid().ToString() + ".pdf";
            string mimeType = MimeMapping.MimeUtility.GetMimeMapping(blobName);

            var blobUrl = await UploadToBlobStorage(connectionString, containerName, blobName, pdfContent.Content, mimeType);

            // Return the URL of the stored PDF
            return new OkObjectResult($"PDF stored at: {blobUrl}");
        }

        private static async Task<string> UploadToBlobStorage(string connectionString, string containerName, string blobName, byte[] content, string fileMimeType)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);

            if (await container.CreateIfNotExistsAsync())
            {
                await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            if (blobName != null && content != null)
            {
                var blob = container.GetBlockBlobReference(blobName);
                blob.Properties.ContentType = fileMimeType;
                await blob.UploadFromByteArrayAsync(content, 0, content.Length);
                return blob.Uri.AbsoluteUri;
            }

            return "";
        }
    }
}