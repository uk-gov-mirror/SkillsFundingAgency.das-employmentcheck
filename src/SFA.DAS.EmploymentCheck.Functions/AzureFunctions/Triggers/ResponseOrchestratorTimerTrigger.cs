using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Orchestrators;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Triggers
{
    public static class ResponseOrchestratorTimerTrigger
    {
        private const string InstanceIdPrefix = "Response-";

        [Function(nameof(ResponseOrchestratorTimerTriggerTask))]
        public static async Task ResponseOrchestratorTimerTriggerTask(
            [TimerTrigger("%ResponseOrchestratorSchedule%")] object timerIgnored,
            [DurableClient] DurableTaskClient starter,
            FunctionContext context)
        {
            var log = context.GetLogger(nameof(ResponseOrchestratorTimerTrigger));
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
                log.LogInformation("ResponseOrchestrator already running, not starting a new instance.");
                return;
            }

            var options = new StartOrchestrationOptions
            {
                InstanceId = $"{InstanceIdPrefix}{Guid.NewGuid()}"
            };

            await starter.ScheduleNewOrchestrationInstanceAsync(new TaskName(nameof(ResponseOrchestrator)), input: null, options: options);
        }
    }
}