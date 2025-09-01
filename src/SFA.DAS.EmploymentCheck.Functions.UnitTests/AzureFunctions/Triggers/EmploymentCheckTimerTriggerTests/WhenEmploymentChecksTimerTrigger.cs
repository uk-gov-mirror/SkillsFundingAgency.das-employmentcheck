using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Orchestrators;
using SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Triggers;
using SFA.DAS.EmploymentCheck.Functions.UnitTests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmploymentCheck.Functions.UnitTests.AzureFunctions.Triggers.EmploymentCheckTimerTriggerTests
{
    public class WhenEmploymentChecksTimerTrigger
    {
        private static async IAsyncEnumerable<OrchestrationMetadata> Empty() { yield break; }

        [Test]
        public async Task Then_Runs_Both_Orchestrators_When_No_Instances()
        {
            var client = new Mock<DurableTaskClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAllInstancesAsync(It.IsAny<OrchestrationQuery>()))
                  .Returns(new TestAsyncPageable<OrchestrationMetadata>(Empty()));
            client.Setup(c => c.ScheduleNewOrchestrationInstanceAsync(
                    It.Is<TaskName>(t => t.Name == nameof(CreateEmploymentCheckCacheRequestsOrchestrator)),
                    null,
                    It.IsAny<StartOrchestrationOptions>(),
                    default))
                  .ReturnsAsync(Guid.NewGuid().ToString());
            client.Setup(c => c.ScheduleNewOrchestrationInstanceAsync(
                    It.Is<TaskName>(t => t.Name == nameof(ProcessEmploymentCheckRequestsOrchestrator)),
                    null,
                    It.IsAny<StartOrchestrationOptions>(),
                    default))
                  .ReturnsAsync(Guid.NewGuid().ToString());

            var logger = new Mock<ILogger>().Object;
            var context = new TestFunctionContext(logger);

            await EmploymentChecksTimerTrigger.EmploymentChecksTimerTriggerTask(new object(), client.Object, context);

            client.VerifyAll();
        }
    }
}
