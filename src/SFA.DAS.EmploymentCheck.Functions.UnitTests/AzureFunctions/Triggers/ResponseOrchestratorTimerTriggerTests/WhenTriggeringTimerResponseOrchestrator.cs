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

namespace SFA.DAS.EmploymentCheck.Functions.UnitTests.AzureFunctions.Triggers.ResponseOrchestratorTimerTriggerTests
{
    public class WhenTriggeringTimerResponseOrchestrator
    {
        private static async IAsyncEnumerable<OrchestrationMetadata> Empty() { yield break; }

        [Test]
        public async Task Then_Schedules_Response_Orchestrator_When_Not_Running()
        {
            var client = new Mock<DurableTaskClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAllInstancesAsync(It.IsAny<OrchestrationQuery>()))
                .Returns(new TestAsyncPageable<OrchestrationMetadata>(Empty()));
            client.Setup(c => c.ScheduleNewOrchestrationInstanceAsync(
                    It.Is<TaskName>(t => t.Name == nameof(ResponseOrchestrator)),
                    null,
                    It.IsAny<StartOrchestrationOptions>(),
                    default))
                .ReturnsAsync(Guid.NewGuid().ToString());

            var logger = new Mock<ILogger>().Object;
            var context = new TestFunctionContext(logger);

            await ResponseOrchestratorTimerTrigger.ResponseOrchestratorTimerTriggerTask(new object(), client.Object, context);

            client.VerifyAll();
        }
    }
}