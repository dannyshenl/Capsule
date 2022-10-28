using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
namespace XBMall.Server.Image
{
    class Program
    {
        static Task Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile($"configs/appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json");
            var config = configurationBuilder.Build();
            var maxRequestBodySize = config["MaxRequestBodySize"] ?? "30";
            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .ConfigureLogging((webHostBuilderContext, loggingBuilder) =>
                {
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddFilter((logLevel) =>
                    {
                        if (webHostBuilderContext.HostingEnvironment.EnvironmentName == Environments.Development)
                        {
                            return true;
                        }
                        else
                        {
                            return logLevel >= LogLevel.Error;
                        }
                    });
                })
                .UseStartup<Startup>()
                .UseKestrel(x => x.Limits.MaxRequestBodySize = int.Parse(maxRequestBodySize) * 1024 * 1024)
                .Build()
                .RunAsync();
        }
    }
}
