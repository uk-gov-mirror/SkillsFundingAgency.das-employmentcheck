using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Orchestrators;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Triggers
{
    public class TriggerHelper : ITriggerHelper
    {
        private readonly string _createRequestsOrchestratorName;
        private readonly string _processRequestsOrchestratorName;
        private readonly string _createRequestsOrchestratorTriggerName;
        private readonly string _processRequestsOrchestratorTriggerName;

        private const string OrchestratorHttpTriggerNameSuffix = "HttpTrigger";

        public TriggerHelper(
            string createRequestsOrchestratorName = nameof(CreateEmploymentCheckCacheRequestsOrchestrator),
            string processRequestsOrchestratorName = nameof(ProcessEmploymentCheckRequestsOrchestrator))
        {
            _createRequestsOrchestratorName = createRequestsOrchestratorName;
            _processRequestsOrchestratorName = processRequestsOrchestratorName;
            _createRequestsOrchestratorTriggerName = $"{createRequestsOrchestratorName}{OrchestratorHttpTriggerNameSuffix}";
            _processRequestsOrchestratorTriggerName = $"{processRequestsOrchestratorName}{OrchestratorHttpTriggerNameSuffix}";
        }

        public async Task<OrchestrationMetadata?> GetRunningInstances(string orchestratorName, string instanceIdPrefix, DurableTaskClient starter, ILogger log)
        {
            var query = new OrchestrationQuery
            {
                InstanceIdPrefix = instanceIdPrefix,
                Statuses = new[]
                {
                    OrchestrationRuntimeStatus.Pending,
                    OrchestrationRuntimeStatus.Running,
                    OrchestrationRuntimeStatus.ContinuedAsNew
                }
            };

            await foreach (var instance in starter.GetAllInstancesAsync(query))
            {
                return instance;
            }

            return null;
        }

        public async Task<HttpResponseData> StartOrchestrator(
            HttpRequestData req,
            DurableTaskClient starter,
            ILogger log,
            ITriggerHelper triggerHelper,
            string orchestratorName,
            string triggerName)
        {
            var existingInstances = await triggerHelper.GetRunningInstances(orchestratorName, triggerName, starter, log);
            if (existingInstances != null)
            {
                var conflict = req.CreateResponse(HttpStatusCode.Conflict);
                await conflict.WriteStringAsync($"An instance of {orchestratorName} is already running.");
                return conflict;
            }

            var options = new StartOrchestrationOptions
            {
                InstanceId = $"{triggerName}-{Guid.NewGuid()}"
            };

            var instanceId = await starter.ScheduleNewOrchestrationInstanceAsync(new TaskName(orchestratorName), input: null, options: options);
            return starter.CreateCheckStatusResponse(req, instanceId, HttpStatusCode.Accepted);
        }

        public async Task<HttpResponseData> StartTheEmploymentCheckOrchestrators(
            HttpRequestData req,
            DurableTaskClient starter,
            ILogger log,
            ITriggerHelper triggerHelper)
        {
            var first = await triggerHelper.StartOrchestrator(req, starter, log, triggerHelper, _createRequestsOrchestratorName, _createRequestsOrchestratorTriggerName);
            if (first.StatusCode != HttpStatusCode.Accepted)
            {
                return first;
            }

            var second = await triggerHelper.StartOrchestrator(req, starter, log, triggerHelper, _processRequestsOrchestratorName, _processRequestsOrchestratorTriggerName);
            if (second.StatusCode != HttpStatusCode.Accepted)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteStringAsync($"Unable to start {_processRequestsOrchestratorName} orchestrator.");
                return error;
            }

            return first;
        }
    }
}
