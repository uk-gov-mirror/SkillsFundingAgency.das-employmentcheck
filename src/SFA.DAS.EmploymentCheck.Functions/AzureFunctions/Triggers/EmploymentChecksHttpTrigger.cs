using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Triggers
{
    public static class EmploymentChecksHttpTrigger
    {
        [Function(nameof(EmploymentChecksHttpTrigger))]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "orchestrators/EmploymentChecksOrchestrator")] HttpRequestData req,
            [DurableClient] DurableTaskClient starter,
            FunctionContext context)
        {
            var log = context.GetLogger(nameof(EmploymentChecksHttpTrigger));
            ITriggerHelper triggerHelper = new TriggerHelper();
            return await triggerHelper.StartTheEmploymentCheckOrchestrators(req, starter, log, triggerHelper);
        }
    }
}