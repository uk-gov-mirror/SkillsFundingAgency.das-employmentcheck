using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Extensions;
using SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Orchestrators;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Triggers
{
    public static class EmploymentChecksTimerTrigger
    {
        [Function(nameof(EmploymentChecksTimerTrigger))]
        public static async Task EmploymentChecksTimerTriggerTask(
            [TimerTrigger("%EmploymentCheckTriggerTime%")] object timerIgnored,
            [DurableClient] DurableTaskClient starter,
            FunctionContext context)
        {
            await starter.StartIfNotRunning(nameof(CreateEmploymentCheckCacheRequestsOrchestrator));
            await starter.StartIfNotRunning(nameof(ProcessEmploymentCheckRequestsOrchestrator));
        }
    }
}