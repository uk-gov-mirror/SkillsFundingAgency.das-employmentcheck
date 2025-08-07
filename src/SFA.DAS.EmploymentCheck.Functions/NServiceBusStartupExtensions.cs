using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using System;
using NServiceBus.Newtonsoft.Json;
using SFA.DAS.EmploymentCheck.Infrastructure.Configuration;

public static class NServiceBusStartupExtensions
{
    public static IServiceCollection AddNServiceBus(this IServiceCollection services, ApplicationSettings appSettings)
    {
        var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmploymentCheck");

        if (appSettings.NServiceBusConnectionString.Equals("UseLearningEndpoint=true", StringComparison.OrdinalIgnoreCase))
        {
            var transport = new LearningTransport();
            endpointConfiguration.UseTransport(transport);
        }
        else
        {
            // ✅ FIXED: Use constructor with connection string
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