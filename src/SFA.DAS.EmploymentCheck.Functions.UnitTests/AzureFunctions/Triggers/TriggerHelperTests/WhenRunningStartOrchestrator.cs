using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Triggers;
using SFA.DAS.EmploymentCheck.Functions.UnitTests.TestHelpers;

namespace SFA.DAS.EmploymentCheck.Functions.UnitTests.AzureFunctions.Triggers.TriggerHelperTests
{
    public class WhenRunningStartOrchestrator
    {
        private static async IAsyncEnumerable<OrchestrationMetadata> Empty() { yield break; }
        private static async IAsyncEnumerable<OrchestrationMetadata> Single() { yield return default; }

        [Test]
        public async Task Returns_Conflict_When_Already_Running()
        {
            var helper = new TriggerHelper();
            var client = new Mock<DurableTaskClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAllInstancesAsync(It.IsAny<OrchestrationQuery>()))
                  .Returns(new TestAsyncPageable<OrchestrationMetadata>(Single()));

            var logger = new Mock<ILogger>().Object;
            var ctx = new TestFunctionContext(logger);
            var req = new TestHttpRequestData(ctx, "POST");

            HttpResponseData res = await helper.StartOrchestrator(req, client.Object, logger, helper, "orchestrator", "trigger");

            Assert.AreEqual(HttpStatusCode.Conflict, res.StatusCode);
        }

        [Test]
        public async Task Returns_Accepted_When_Not_Running()
        {
            var helper = new TriggerHelper();
            var client = new Mock<DurableTaskClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAllInstancesAsync(It.IsAny<OrchestrationQuery>()))
                  .Returns(new TestAsyncPageable<OrchestrationMetadata>(Empty()));
            client.Setup(c => c.ScheduleNewOrchestrationInstanceAsync(
                    It.IsAny<TaskName>(), null, It.IsAny<StartOrchestrationOptions>(), default))
                  .ReturnsAsync("id");

            var logger = new Mock<ILogger>().Object;
            var ctx = new TestFunctionContext(logger);
            var req = new TestHttpRequestData(ctx, "POST");

            HttpResponseData res = await helper.StartOrchestrator(req, client.Object, logger, helper, "orchestrator", "trigger");

            Assert.AreEqual(HttpStatusCode.Accepted, res.StatusCode);
        }
    }
}
