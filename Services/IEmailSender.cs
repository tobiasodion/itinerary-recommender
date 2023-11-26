using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace az_function
{
    public interface IEmailSender
    {
        Task SendEmail(SendEmailRequest sendEmailRequest, ILogger log);
    }
}