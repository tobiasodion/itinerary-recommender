using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;

namespace az_function
{
    public class BlobUploader : IFileUploader
    {
        private readonly IConfiguration _configuration;

        public BlobUploader(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> UploadFile(PdfContent pdfContent, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Upload the PDF to Azure Blob Storage
            string connectionString = _configuration["AzureWebJobsStorage"];
            string containerName = "ia-pdf";
            string blobName = Guid.NewGuid().ToString() + ".pdf";
            string mimeType = MimeMapping.MimeUtility.GetMimeMapping(blobName);

            try
            {
                var blobUrl = await UploadToBlobStorage(connectionString, containerName, blobName, pdfContent.Content, mimeType);

                if (blobUrl != String.Empty)
                {
                    log.LogInformation($"PDF Stored at: {blobUrl}");
                }
                else
                {
                    log.LogError($"Something went wrong during blob upload");
                }

                // Return the URL of the stored PDF
                return blobUrl;
            }
            catch (Exception ex)
            {
                log.LogError($"Error - {ex.Message}");
                throw;
            }
        }

        private async Task<string> UploadToBlobStorage(string connectionString, string containerName, string blobName, byte[] content, string fileMimeType)
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

            return String.Empty;
        }
    }
}