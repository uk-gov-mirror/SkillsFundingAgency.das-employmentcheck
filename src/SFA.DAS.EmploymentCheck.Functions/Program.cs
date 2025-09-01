using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EmploymentCheck.Commands;
using SFA.DAS.EmploymentCheck.Functions;
using SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Telemetry;
using SFA.DAS.EmploymentCheck.Infrastructure.Configuration;
using SFA.DAS.EmploymentCheck.Queries;
using System.IO;

var host = new HostBuilder()
    .ConfigureAppConfiguration((ctx, cfg) =>
    {
        cfg.AddEnvironmentVariables();
        cfg.SetBasePath(Directory.GetCurrentDirectory());
        cfg.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

        var built = cfg.Build();
        var environmentName = built["EnvironmentName"] ?? "LOCAL";
        var isLocalOrDev = environmentName.Equals("LOCAL", System.StringComparison.OrdinalIgnoreCase)
                           || environmentName.Equals("DEV", System.StringComparison.OrdinalIgnoreCase);

        if (!isLocalOrDev)
        {
            cfg.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = new[]
                {
                    "SFA.DAS.EmploymentCheck.Functions",
                    "SFA.DAS.EmploymentCheck"
                };
                options.EnvironmentName = environmentName;
                options.PreFixConfigurationKeys = false;
            });
        }
    })
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((ctx, services) =>
    {
        var envName = System.Environment.GetEnvironmentVariable("EnvironmentName") ?? "LOCAL";
        var config = ctx.Configuration;

        var localRoot = System.Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot");
        var azureRoot = $"{System.Environment.GetEnvironmentVariable("HOME")}/site/wwwroot";
        var applicationDirectory = localRoot ?? azureRoot ?? Directory.GetCurrentDirectory();
        services.AddNLog(applicationDirectory, envName);

        services.AddOptions();

        services.Configure<ApiRetryOptions>(config.GetSection("ApiRetryOptions"));
        services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ApiRetryOptions>>().Value);

        services.Configure<EmployerAccountApiConfiguration>(config.GetSection("AccountsInnerApi"));
        services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<EmployerAccountApiConfiguration>>().Value);

        services.Configure<HmrcApiConfiguration>(config.GetSection("HmrcApiSettings"));
        services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<HmrcApiConfiguration>>().Value);

        services.Configure<HmrcApiRateLimiterOptions>(config.GetSection("HmrcApiRateLimiterOptions"));
        services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<HmrcApiRateLimiterOptions>>().Value);

        services.Configure<TokenServiceApiClientConfiguration>(config.GetSection("TokenService"));
        services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<TokenServiceApiClientConfiguration>>().Value);

        services.Configure<ApplicationSettings>(config.GetSection("ApplicationSettings"));
        var appSettings = config.GetSection("ApplicationSettings").Get<ApplicationSettings>() ?? new ApplicationSettings();

        services.AddSingleton<IHmrcApiTelemetrySanitizer, HmrcApiTelemetrySanitizer>();
        services.AddSingleton<ILearnerDataTelemetrySanitizer, LearnerDataTelemetrySanitizer>();
        services.AddSingleton<ITelemetryInitializer, TelemetryIntializer>();

        services
            .AddMemoryCache()
            .AddCommandServices()
            .AddQueryServices()
            .AddApprenticeshipLevyApiClient()
            .AddEmploymentCheckService(envName)
            .AddPersistenceServices()
            .AddNServiceBus(appSettings);
    })
    .Build();

await host.RunAsync();
