using System;
using System.Text.RegularExpressions;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.Newtonsoft.Json;
using NServiceBus.Transport.AzureServiceBus;
using SFA.DAS.EmploymentCheck.Infrastructure.Configuration;

public static class NServiceBusStartupExtensions
{
    public static IServiceCollection AddNServiceBus(this IServiceCollection services, ApplicationSettings appSettings)
    {
        var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmploymentCheck");

        var raw = appSettings.NServiceBusConnectionString?.Trim();
        if (string.Equals(raw, "UseLearningEndpoint=true", StringComparison.OrdinalIgnoreCase))
        {
            endpointConfiguration.UseTransport<LearningTransport>();
        }
        else
        {
            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            ConfigureAzureServiceBusTransport(transport, raw);
        }

        endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo("error");

        var endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        services.AddSingleton<IMessageSession>(endpointInstance);

        return services;
    }

    private static void ConfigureAzureServiceBusTransport(TransportExtensions<AzureServiceBusTransport> transport, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException(
                "ApplicationSettings:NServiceBusConnectionString is required. " +
                "Set 'UseLearningEndpoint=true' locally or 'Endpoint=sb://<ns>.servicebus.windows.net/' in Azure.");

        var hasKey = value.Contains("SharedAccessKey", StringComparison.OrdinalIgnoreCase)
                     || value.Contains("SharedAccessSignature", StringComparison.OrdinalIgnoreCase);

        var m = Regex.Match(value, @"Endpoint=sb:\/\/(?<host>[^\/;]+)", RegexOptions.IgnoreCase);
        if (m.Success && !hasKey)
        {
            transport.CustomTokenCredential(m.Groups["host"].Value.Trim(), new DefaultAzureCredential());
            return;
        }

        if (!value.Contains(";") && value.Contains(".servicebus.windows.net", StringComparison.OrdinalIgnoreCase))
        {
            var fq = value.Replace("sb://", "", StringComparison.OrdinalIgnoreCase).TrimEnd('/');
            transport.CustomTokenCredential(fq, new DefaultAzureCredential());
            return;
        }

        transport.ConnectionString(value);
    }
}
