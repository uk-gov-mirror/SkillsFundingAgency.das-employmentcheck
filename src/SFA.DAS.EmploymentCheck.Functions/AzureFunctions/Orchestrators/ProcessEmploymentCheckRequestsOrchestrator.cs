using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmploymentCheck.Data.Models;
using SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Activities;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Orchestrators
{
    public class ProcessEmploymentCheckRequestsOrchestrator
    {
        private readonly ILogger<ProcessEmploymentCheckRequestsOrchestrator> _logger;

        public ProcessEmploymentCheckRequestsOrchestrator(ILogger<ProcessEmploymentCheckRequestsOrchestrator> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ProcessEmploymentCheckRequestsOrchestrator))]
        public async Task ProcessEmploymentCheckRequestsOrchestratorTask([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            EmploymentCheckCacheRequest[] employmentCheckRequests;
            do
            {
                employmentCheckRequests = await context.CallActivityAsync<EmploymentCheckCacheRequest[]>(nameof(GetEmploymentCheckCacheRequestActivity), null);
                await ProcessEmploymentCheckRequest(context, employmentCheckRequests);
            }
            while (employmentCheckRequests != null);
        }

        private static async Task ProcessEmploymentCheckRequest(TaskOrchestrationContext context, EmploymentCheckCacheRequest[] employmentCheckRequests)
        {
            if (employmentCheckRequests == null || !employmentCheckRequests.Any())
            {
                return;
            }

            var getEmploymentStatusTasks = employmentCheckRequests.Select(request =>
                context.CallActivityAsync<EmploymentCheckCacheRequest>(nameof(GetHmrcLearnerEmploymentStatusActivity), request));

            var completedRequests = await Task.WhenAll(getEmploymentStatusTasks);
            await context.CallActivityAsync(nameof(AbandonRelatedRequestsActivity), completedRequests);
        }
    }
}
