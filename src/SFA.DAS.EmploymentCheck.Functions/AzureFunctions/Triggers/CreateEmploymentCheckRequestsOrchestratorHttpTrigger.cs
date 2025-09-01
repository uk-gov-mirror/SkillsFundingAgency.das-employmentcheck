using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Orchestrators;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Triggers
{
    public static class CreateEmploymentCheckRequestsOrchestratorHttpTrigger
    {
        private const string InstanceIdPrefix = "CreateEmploymentCheck-";

        [Function(nameof(CreateEmploymentCheckRequestsOrchestratorHttpTrigger))]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "orchestrators/CreateApprenticeEmploymentChecksOrchestrator")] HttpRequestData req,
            [DurableClient] DurableTaskClient starter,
            FunctionContext context)
        {
            var query = new OrchestrationQuery
            {
                InstanceIdPrefix = InstanceIdPrefix,
                Statuses = new[]
                {
                    OrchestrationRuntimeStatus.Pending,
                    OrchestrationRuntimeStatus.Running,
                    OrchestrationRuntimeStatus.ContinuedAsNew
                }
            };

            await foreach (var _ in starter.GetAllInstancesAsync(query))
            {
                var conflict = req.CreateResponse(HttpStatusCode.Conflict);
                await conflict.WriteStringAsync("An instance of CreateEmploymentCheckCacheRequestsOrchestrator is already running.");
                return conflict;
            }

            var options = new StartOrchestrationOptions
            {
                InstanceId = $"{InstanceIdPrefix}{Guid.NewGuid()}"
            };

            var instanceId = await starter.ScheduleNewOrchestrationInstanceAsync(new TaskName(nameof(CreateEmploymentCheckCacheRequestsOrchestrator)), input: null, options: options);
            return starter.CreateCheckStatusResponse(req, instanceId, HttpStatusCode.Accepted);
        }
    }
}
