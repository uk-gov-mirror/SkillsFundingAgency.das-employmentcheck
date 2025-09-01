using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.HouseKeeping
{
    public class CleanupWorkflowsTimer
    {
        [Function(nameof(CleanupOldWorkflows))]
        public async Task CleanupOldWorkflows(
            [TimerTrigger("%CleanupOldWorkflowsTriggerTime%")] object timerIgnored,
            [DurableClient] DurableTaskClient orchestrationClient,
            FunctionContext context)
        {
            var log = context.GetLogger(nameof(CleanupWorkflowsTimer));

            var createdTimeTo = DateTimeOffset.UtcNow.AddDays(-1);
            var createdTimeFrom = createdTimeTo.AddDays(-7);

            var statuses = new[]
            {
                OrchestrationRuntimeStatus.Completed,
                OrchestrationRuntimeStatus.Canceled,
                OrchestrationRuntimeStatus.ContinuedAsNew,
                OrchestrationRuntimeStatus.Failed,
                OrchestrationRuntimeStatus.Terminated
            };

            await orchestrationClient.PurgeInstancesAsync(createdTimeFrom, createdTimeTo, statuses);
            log.LogInformation("Scheduled cleanup complete.");
        }
    }
}