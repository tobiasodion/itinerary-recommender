using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;
using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Filters;

namespace az_function
{
    public class SmtpEmailClient : IEmailClient
    {
        private readonly string _senderEmail;
        private readonly string _senderPassword;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        public SmtpEmailClient(string senderEmail, string senderPassword, string smtpHost, int smtpPort)
        {
            _senderEmail = senderEmail;
            _senderPassword = senderPassword;
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
        }
        public async Task SendEmail(SendEmailRequest sendEmailRequest, ILogger log)
        {
            // Recipient email
            var recipientEmail = sendEmailRequest.RecipientEmail;

            // Create a new MailMessage
            using (MailMessage mailMessage = new MailMessage(_senderEmail, recipientEmail)
            {
                Subject = sendEmailRequest.Subject,
                Body = sendEmailRequest.Body
            })
            {
                // Create a new SmtpClient
                using (SmtpClient smtpClient = new SmtpClient(_smtpHost, _smtpPort)
                {
                    Credentials = new NetworkCredential(_senderEmail, _senderPassword),
                    EnableSsl = true
                })
                {
                    try
                    {
                        // Send the email
                        await smtpClient.SendMailAsync(mailMessage);
                        log.LogInformation("Email sent successfully.");
                    }
                    catch (Exception ex)
                    {
                        log.LogError($"Error: {ex.Message}");
                        throw;
                    }
                }
            }
        }
    }
}