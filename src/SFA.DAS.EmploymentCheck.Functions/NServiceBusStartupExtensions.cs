using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.Newtonsoft.Json;
using SFA.DAS.EmploymentCheck.Infrastructure.Configuration;
using System;
using System.IO;

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

        if (appSettings.NServiceBusConnectionString.Equals("UseLearningEndpoint=true", StringComparison.OrdinalIgnoreCase))
        {
            var rootDir = Directory.GetCurrentDirectory();
            var baseDir = rootDir.Contains("src")
                ? rootDir.Substring(0, rootDir.IndexOf("src", StringComparison.OrdinalIgnoreCase))
                : rootDir;

            var learningTransportDir = Path.Combine(baseDir, "src", ".learningtransport");

            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            transport.StorageDirectory(learningTransportDir);
        }
        else
        {
            var transport = new AzureServiceBusTransport(appSettings.NServiceBusConnectionString);
            endpointConfiguration.UseTransport(transport);
        }

        endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo("error");

        var endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        services.AddSingleton<IMessageSession>(endpointInstance);

        return services;
    }
}