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

namespace SFA.DAS.EmploymentCheck.Functions.UnitTests.AzureFunctions.Triggers.EmploymentCheckHttpTriggerTests
{
    public class WhenTriggeringHttpEmploymentChecks
    {
        private static async IAsyncEnumerable<OrchestrationMetadata> Empty() { yield break; }

        [Test]
        public async Task Returns_Accepted_And_Schedules_Both()
        {
            var client = new Mock<DurableTaskClient>(MockBehavior.Strict);

            client.SetupSequence(c => c.GetAllInstancesAsync(It.IsAny<OrchestrationQuery>()))
                .Returns(new TestAsyncPageable<OrchestrationMetadata>(Empty()))
                .Returns(new TestAsyncPageable<OrchestrationMetadata>(Empty()));

            client.Setup(c => c.ScheduleNewOrchestrationInstanceAsync(
                    It.IsAny<TaskName>(), null, It.IsAny<StartOrchestrationOptions>(), default))
                .ReturnsAsync("id");

            var logger = new Mock<ILogger>().Object;
            var ctx = new TestFunctionContext(logger);
            var req = new TestHttpRequestData(ctx, "POST");

            HttpResponseData res = await EmploymentChecksHttpTrigger.HttpStart(req, client.Object, ctx);

            Assert.AreEqual(HttpStatusCode.Accepted, res.StatusCode);
        }
    }
}