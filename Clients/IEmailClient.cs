using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace az_function
{
    public interface IEmailClient
    {
        Task SendEmail(SendEmailRequest sendEmailRequest, ILogger log);
    }
}