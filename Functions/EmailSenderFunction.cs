using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace az_function
{
    public class EmailSenderFunction
    {
        private readonly IEmailSender _emailSender;

        public EmailSenderFunction(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [FunctionName("ItineraryGeneratorJob_SendEmail")]
        public async Task GenerateItinerary([ActivityTrigger] SendEmailRequest sendEmailRequest, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            await _emailSender.SendEmail(sendEmailRequest, log);
        }
    }
}