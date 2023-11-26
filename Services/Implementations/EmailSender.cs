using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace az_function
{
    public class EmailSender : IEmailSender
    {
        private readonly IEmailClient _smtpEmailClient;

        public EmailSender(IEmailClient smtpEmailClient)
        {
            _smtpEmailClient = smtpEmailClient;
        }

        public async Task SendEmail(SendEmailRequest sendEmailRequest, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                await _smtpEmailClient.SendEmail(sendEmailRequest, log);
                log.LogInformation($"Email sent Successfully - Details: {sendEmailRequest}");
            }
            catch (Exception ex)
            {
                log.LogError($"Error - {ex.Message}");
                throw;
            }
        }
    }
}