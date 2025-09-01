using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Extensions
{
    public static class DurableOrchestrationClientExtensions
    {
        public static IEnumerable<OrchestrationRuntimeStatus> RuntimeStatuses => new[]
        {
            OrchestrationRuntimeStatus.Pending,
            OrchestrationRuntimeStatus.Running,
            OrchestrationRuntimeStatus.ContinuedAsNew
        };

        public static async Task StartIfNotRunning(this DurableTaskClient starter, string orchestrator)
        {
            var query = new OrchestrationQuery
            {
                InstanceIdPrefix = orchestrator,
                Statuses = RuntimeStatuses
            };

            await foreach (var _ in starter.GetAllInstancesAsync(query))
            {
                return;
            }

            var options = new StartOrchestrationOptions
            {
                InstanceId = $"{orchestrator}-{Guid.NewGuid()}"
            };

            await starter.ScheduleNewOrchestrationInstanceAsync(new TaskName(orchestrator), input: null, options: options);
        }
    }
}