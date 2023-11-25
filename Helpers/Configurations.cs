using System;
using Microsoft.Extensions.Configuration;

namespace az_function
{
    public static class Configurations
    {
        public static IConfigurationRoot GetConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

           return configuration;
        }
    }
}