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
    public static class PdfGenerator
    {
        [FunctionName("Chaining_GeneratePdf")]
        public static async Task<IActionResult> GeneratePdf([ActivityTrigger] string request, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Generate PDF from the input string
            byte[] pdfBytes = GeneratePdf(request);

            // Upload the PDF to Azure Blob Storage
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=tobiasodionia;AccountKey=I6LkSh1JAnsyDFQmzfYekc6vP07/if7XVZAKKJQ0Mojjhwcusas3AEN30XV9muojtPRkasfCQwva+AStUQjZeg==;EndpointSuffix=core.windows.net";
            string containerName = "ia-pdf";
            string blobName = Guid.NewGuid().ToString() + ".pdf";
            string mimeType = MimeMapping.MimeUtility.GetMimeMapping(blobName);

            var blobUrl = await UploadToBlobStorage(connectionString, containerName, blobName, pdfBytes, mimeType);

            // Return the URL of the stored PDF
            return new OkObjectResult($"PDF stored at: {blobUrl}");
        }

        private static byte[] GeneratePdf(string content)
        {
            // Set the global font resolver
            GlobalFontSettings.FontResolver = new CustomFontResolver();

            using (MemoryStream stream = new MemoryStream())
            {
                using (PdfDocument pdfDocument = new PdfDocument())
                {
                    PdfPage page = pdfDocument.AddPage();
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XFont font = new XFont("Arial", 12);
                    XRect rect = new XRect(10, 10, 200, page.Height);

                    //gfx.DrawString(content, font, XBrushes.Black, rect, XStringFormats.TopLeft);
                    DrawWrappedText(gfx, font, content, rect);
                    pdfDocument.Save(stream);
                }

                return stream.ToArray();
            }
        }

        private static void DrawWrappedText(XGraphics gfx, XFont font, string text, XRect rect)
        {
           // Split the text into words
            string[] words = text.Split(' ');

            // Initialize variables
            string currentLine = "";
            double currentLineLength = 0;

            // Iterate through each word
            foreach (string word in words)
            {
                // Measure the width of the current line with the new word
                double wordWidth = gfx.MeasureString(word, font).Width;

                // Check if adding the word to the current line exceeds the rectangle width
                if (currentLineLength + wordWidth <= rect.Width)
                {
                    // Add the word to the current line
                    currentLine += word + " ";
                    currentLineLength += wordWidth;
                }
                else
                {
                    // Draw the current line and reset variables for the new line
                    gfx.DrawString(currentLine.Trim(), font, XBrushes.Black, rect, XStringFormats.TopLeft);
                    rect = new XRect(rect.Left, rect.Top + font.Height, rect.Width, rect.Height - font.Height);
                    currentLine = word + " ";
                    currentLineLength = wordWidth;
                }
            }

            // Draw the remaining text
            gfx.DrawString(currentLine.Trim(), font, XBrushes.Black, rect, XStringFormats.TopLeft);
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