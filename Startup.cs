using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(az_function.Startup))]

namespace az_function
{
    public class Startup : FunctionsStartup
    {
        IConfiguration configuration;
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();

            configuration = builder.ConfigurationBuilder.Build();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(configuration);
            builder.Services.AddTransient<IGptClient, GptClient>();


            builder.Services.AddTransient<IEmailClient>(provider =>
            {
                var senderEmail = configuration["EMAIL_ACCOUNT"];
                var senderPassword = configuration["EMAIL_PASSWORD"];
                var smtpHost = configuration["SMTP_HOST"];
                var smtpPort = int.Parse(configuration["SMTP_PORT"]);

                return new SmtpEmailClient(senderEmail, senderPassword, smtpHost, smtpPort);
            });
            // Register your services here
            builder.Services.AddTransient<IFileUploader, BlobUploader>();
            builder.Services.AddTransient<ILLMCompletionGenerator, ItineraryGenerator>();
            builder.Services.AddTransient<IFileGenerator, PdfGenerator>();

            builder.Services.AddTransient<IEmailSender>(provider =>
            {
                // Resolve the IEmailClient dependency
                var emailClient = provider.GetRequiredService<IEmailClient>();

                return new EmailSender(emailClient);
            });
        }
    }
}