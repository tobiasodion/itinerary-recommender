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

            // Upload the PDF to Azure Blob Storage
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=tobiasodionia;AccountKey=M+qIZ5h+J/Go82Z9Kbg1bbT/9Qc1veG6u3hBNOVQ0NxX0v45zWW5m4jSOefBAsrzh/grXliG4gPw+ASt4xMhYA==;EndpointSuffix=core.windows.net";
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

            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

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