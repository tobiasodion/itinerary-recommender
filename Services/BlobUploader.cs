using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace az_function
{
    public static class BlobUploader
    {
        public static async Task<IActionResult> UploadBlob(PdfContent pdfContent, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var configurations = Configurations.GetConfiguration();

            // Upload the PDF to Azure Blob Storage
            string connectionString = configurations["AzureWebJobsStorage"];
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