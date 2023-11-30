using System;
using System.IO;
using System.Reflection;
using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

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
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly(), opts =>
            {
                opts.AddCodeParameter = true;
                opts.Documents = new[] {
                    new SwaggerDocument {
                        Name = "v1",
                            Title = "Swagger document",
                            Description = "Integrate Swagger UI With Azure Functions",
                            Version = "v2"
                    }
                };
                opts.ConfigureSwaggerGen = x =>
                {
                    x.CustomOperationIds(apiDesc =>
                    {
                        return apiDesc.TryGetMethodInfo(out MethodInfo mInfo) ? mInfo.Name : default(Guid).ToString();
                    });
                };
            });

            builder.Services.AddSingleton(configuration);

            builder.Services.AddTransient<IGptClient>(provider =>
            {
                var apiKey = configuration["GPT_API_KEY"];
                return new GptClient(apiKey);
            });

            builder.Services.AddTransient<IEmailClient>(provider =>
            {
                var senderEmail = configuration["EMAIL_ACCOUNT"];
                var senderPassword = configuration["EMAIL_PASSWORD"];
                var smtpHost = configuration["SMTP_HOST"];
                var smtpPort = int.Parse(configuration["SMTP_PORT"]);

                return new SmtpEmailClient(senderEmail, senderPassword, smtpHost, smtpPort);
            });

            // Register your services here

            builder.Services.AddTransient<ILLMCompletionGenerator>(provider =>
            {
                // Resolve the IEmailClient dependency
                var gptClient = provider.GetRequiredService<IGptClient>();

                return new ItineraryGenerator(gptClient, configuration);
            });

            builder.Services.AddTransient<IFileGenerator, PdfGenerator>();
            builder.Services.AddTransient<IFileUploader, BlobUploader>();

            builder.Services.AddTransient<IEmailSender>(provider =>
            {
                // Resolve the IEmailClient dependency
                var emailClient = provider.GetRequiredService<IEmailClient>();

                return new EmailSender(emailClient);
            });
        }
    }
}