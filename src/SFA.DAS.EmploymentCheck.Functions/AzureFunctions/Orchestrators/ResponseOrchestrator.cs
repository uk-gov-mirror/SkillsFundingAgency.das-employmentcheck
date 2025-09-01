using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Activities;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Orchestrators
{
    public class ResponseOrchestrator
    {
        [Function(nameof(ResponseOrchestrator))]
        public async Task ResponseOrchestratorTask([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var loop = true;
            while (loop)
            {
                var employmentCheck = await context.CallActivityAsync<Data.Models.EmploymentCheck>(nameof(GetResponseEmploymentCheckActivity), null);
                if (employmentCheck != null)
                {
                    await context.CallActivityAsync(nameof(OutputEmploymentCheckResultsActivity), employmentCheck);
                }
                else
                {
                    loop = false;
                }
            }
        }
    }
}