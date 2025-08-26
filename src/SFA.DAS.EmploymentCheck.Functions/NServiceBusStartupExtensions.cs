using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.Newtonsoft.Json;
using SFA.DAS.EmploymentCheck.Infrastructure.Configuration;
using System;
using System.IO;
using Azure.Identity;
using NServiceBus.Transport.AzureServiceBus;

public static class NServiceBusStartupExtensions
{
    public static IServiceCollection AddNServiceBus(this IServiceCollection services, ApplicationSettings appSettings)
    {
        var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmploymentCheck");

        endpointConfiguration.AssemblyScanner()
            .ExcludeAssemblies(
                "Azure.Core.dll",
                "Microsoft.Bcl.AsyncInterfaces.dll",
                "System.Threading.Tasks.Extensions.dll",
                "System.Reactive.Core.dll",
                "System.Reactive.dll",
                "System.Reactive.Linq.dll",
                "System.ClientModel.dll"
            );

        if (appSettings.NServiceBusConnectionString?.Equals("UseLearningEndpoint=true", StringComparison.OrdinalIgnoreCase) == true)
        {
            var rootDir = Directory.GetCurrentDirectory();
            var baseDir = rootDir.Contains("src")
                ? rootDir.Substring(0, rootDir.IndexOf("src", StringComparison.OrdinalIgnoreCase))
                : rootDir;

            var learningTransportDir = Path.Combine(baseDir, "src", ".learningtransport");

            var learning = endpointConfiguration.UseTransport<LearningTransport>();
            learning.StorageDirectory(learningTransportDir);
        }
        else
        {
            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();

            if (appSettings.UseNServiceBusConnectionString || (appSettings.NServiceBusConnectionString?.StartsWith("Endpoint=", StringComparison.OrdinalIgnoreCase) ?? false))
            {
                transport.ConnectionString(appSettings.NServiceBusConnectionString);
            }
            else
            {
                // Default: Managed Identity
                if(string.IsNullOrWhiteSpace(appSettings.NServiceBusConnectionString))
                {
                    throw new InvalidOperationException(
                        "ApplicationSettings:NServiceBusConnectionString must be set to either a full connection string or a namespace when using Managed Identity.");
                }

                transport.CustomTokenCredential(appSettings.NServiceBusConnectionString, new DefaultAzureCredential());
            }
        }

        endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo("error");

        var endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        services.AddSingleton<IMessageSession>(endpointInstance);

        return services;
    }
}
