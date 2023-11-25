using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace az_function
{
    public class PdfGeneratorActivity
    {
        private readonly IFileGenerator _pdfGenerator;

        public PdfGeneratorActivity(IFileGenerator pdfGenerator){
            this._pdfGenerator = pdfGenerator;
        }

        [FunctionName("ItineraryGeneratorJob_GeneratePdfStream")]
        public async Task<byte[]> GeneratePdfStream([ActivityTrigger] string text, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _pdfGenerator.GenerateFileStream(text, log);

            return result;
        }
    }
}