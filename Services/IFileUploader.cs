using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace az_function
{
    public interface IFileUploader
    {
        Task<string> UploadFile(PdfContent pdfContent, ILogger log);
    }
}