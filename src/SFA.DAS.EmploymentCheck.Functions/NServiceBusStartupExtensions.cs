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

    private static void ConfigureAzureServiceBusTransport(
        TransportExtensions<AzureServiceBusTransport> transport,
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException(
                "ApplicationSettings:NServiceBusConnectionString is required. " +
                "Set 'UseLearningEndpoint=true' locally or 'Endpoint=sb://<ns>.servicebus.windows.net/' in Azure.");

        var hasKey = value.IndexOf("SharedAccessKey", StringComparison.OrdinalIgnoreCase) >= 0;
        var hasSas = value.IndexOf("SharedAccessSignature", StringComparison.OrdinalIgnoreCase) >= 0;

        var endpointMatch = Regex.Match(value, @"Endpoint=sb:\/\/(?<host>[^\/;]+)", RegexOptions.IgnoreCase);
        if (endpointMatch.Success && !(hasKey || hasSas))
        {
            var fqdn = endpointMatch.Groups["host"].Value.Trim();
            transport.CustomTokenCredential(fqdn, new DefaultAzureCredential());
            return;
        }

        if (!value.Contains(";") && value.Contains(".servicebus.windows.net", StringComparison.OrdinalIgnoreCase))
        {
            var cleaned = value.Replace("sb://", "", StringComparison.OrdinalIgnoreCase).TrimEnd('/');
            transport.CustomTokenCredential(cleaned, new DefaultAzureCredential());
            return;
        }

        transport.ConnectionString(value);
    }
}
