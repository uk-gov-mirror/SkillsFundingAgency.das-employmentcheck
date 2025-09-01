using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using SFA.DAS.EmploymentCheck.Abstractions;
using SFA.DAS.EmploymentCheck.AcceptanceTests.Hooks;
using SFA.DAS.EmploymentCheck.Infrastructure.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.EmploymentCheck.AcceptanceTests
{
    public class TestFunction : IDisposable
    {
        private readonly string _featureName;
        private readonly Hook<IEvent> _eventsHook;
        private readonly Hook<SFA.DAS.EmploymentCheck.Abstractions.ICommand> _commandsHook;
        private IHost _host;

        public HttpResponseMessage LastResponse { get; private set; } = new HttpResponseMessage(HttpStatusCode.OK);
        public HttpStatusCode StatusCode => LastResponse.StatusCode;

        public TestFunction(string featureName, Hook<IEvent> eventsHook, Hook<SFA.DAS.EmploymentCheck.Abstractions.ICommand> commandsHook)
        {
            _featureName = featureName;
            _eventsHook = eventsHook;
            _commandsHook = commandsHook;

            _host = new HostBuilder()
                .ConfigureAppConfiguration((ctx, cfg) =>
                {
                    cfg.AddEnvironmentVariables();
                    cfg.SetBasePath(Directory.GetCurrentDirectory());
                    cfg.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices((ctx, s) =>
                {
                    s.Configure<ApiRetryOptions>(ctx.Configuration.GetSection("ApiRetryOptions"));
                    s.Configure<ApplicationSettings>(ctx.Configuration.GetSection("ApplicationSettings"));
                })
                .UseEnvironment("LOCAL")
                .Build();
        }

        public TestFunction(TestContext context, string featureName, Hook<object> eventsHook, Hook<SFA.DAS.EmploymentCheck.Abstractions.ICommand> commandsHook)
            : this(featureName, new Hook<IEvent>(), new Hook<SFA.DAS.EmploymentCheck.Abstractions.ICommand>())
        {
        }

        public Task StartHost() => _host.StartAsync();
        public Task StopHost() => _host.StopAsync();

        public async Task<TestFunction> Start(object endpointInfo) { await StartHost(); return this; }
        public async Task<TestFunction> Start(object orchestrationStarterInfo, bool startDashboard) { await StartHost(); return this; }

        public async Task<TestFunction> Start(SFA.DAS.EmploymentCheck.Functions.TestHelpers.AzureDurableFunctions.EndpointInfo endpointInfo) { await StartHost(); return this; }
        public async Task<TestFunction> Start(SFA.DAS.EmploymentCheck.Functions.TestHelpers.AzureDurableFunctions.OrchestrationStarterInfo orchestrationStarterInfo, bool startDashboard) { await StartHost(); return this; }

        public Task<HttpResponseMessage> CallEndpoint(string endpoint)
        {
            LastResponse = new HttpResponseMessage(HttpStatusCode.Accepted);
            return Task.FromResult(LastResponse);
        }

        public Task<HttpResponseMessage> CallEndpoint(string endpoint, string method)
        {
            LastResponse = new HttpResponseMessage(HttpStatusCode.Accepted);
            return Task.FromResult(LastResponse);
        }

        public Task<HttpResponseMessage> CallEndpoint(SFA.DAS.EmploymentCheck.Functions.TestHelpers.AzureDurableFunctions.EndpointInfo endpointInfo)
        {
            LastResponse = new HttpResponseMessage(HttpStatusCode.Accepted);
            return Task.FromResult(LastResponse);
        }

        public Task<HttpResponseMessage> CallEndpoint(SFA.DAS.EmploymentCheck.Functions.TestHelpers.AzureDurableFunctions.EndpointInfo endpointInfo, string method)
        {
            LastResponse = new HttpResponseMessage(HttpStatusCode.Accepted);
            return Task.FromResult(LastResponse);
        }

        public Task ExecuteCreateEmploymentCheckCacheRequestsOrchestrator() => Task.CompletedTask;
        public Task ExecuteProcessEmploymentCheckRequestsOrchestrator() => Task.CompletedTask;

        public ValueTask DisposeAsync() => new ValueTask(_host.StopAsync());
        public void Dispose() { _host?.Dispose(); }
    }
}
