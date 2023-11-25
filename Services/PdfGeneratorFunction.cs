using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Fonts;

namespace az_function
{
    public static class PdfGenerator
    {
        public static async Task<byte[]> GeneratePdfStream(string text, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (GlobalFontSettings.FontResolver == null)
            {
                // Set the global font resolver
                GlobalFontSettings.FontResolver = new CustomFontResolver();
            }

            // Generate PDF from the input string
            byte[] pdfBytes = GetPdfStream(text);

            return pdfBytes;
        }

        private static byte[] GetPdfStream(string content)
        {
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
    }
}