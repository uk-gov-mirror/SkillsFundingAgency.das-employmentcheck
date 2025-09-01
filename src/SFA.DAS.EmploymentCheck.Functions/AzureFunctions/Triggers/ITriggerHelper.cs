using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Microsoft.DurableTask;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Triggers
{
    public interface ITriggerHelper
    {
        Task<OrchestrationMetadata?> GetRunningInstances(
            string orchestratorName,
            string instanceIdPrefix,
            DurableTaskClient starter,
            ILogger log);

        Task<HttpResponseData> StartTheEmploymentCheckOrchestrators(
            HttpRequestData req,
            DurableTaskClient starter,
            ILogger log,
            ITriggerHelper triggerHelper);

        Task<HttpResponseData> StartOrchestrator(
            HttpRequestData req,
            DurableTaskClient starter,
            ILogger log,
            ITriggerHelper triggerHelper,
            string orchestratorName,
            string triggerName);
    }
}