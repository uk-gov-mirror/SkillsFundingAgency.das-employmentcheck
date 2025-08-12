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

        endpointConfiguration
            .UseTransport<LearningTransport>()
            .StorageDirectory(@"C:\dev\das-employmentcheck\.learningtransport");

        if (appSettings.NServiceBusConnectionString.Equals("UseLearningEndpoint=true", StringComparison.OrdinalIgnoreCase))
        {
            //Get transport object and set storage directory via extension method
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            transport.StorageDirectory(Path.Combine(Environment.CurrentDirectory, ".learningtransport"));
        }
        else
        {
            // Use Azure Service Bus
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