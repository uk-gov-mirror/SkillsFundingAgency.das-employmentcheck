using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmploymentCheck.Functions.AzureFunctions.HouseKeeping;
using SFA.DAS.EmploymentCheck.Functions.UnitTests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmploymentCheck.Functions.UnitTests.AzureFunctions.HouseKeeping
{
    public class WhenCleanupWorkflowsTimer
    {
        [Test]
        public async Task Then_Purges_Expected_Statuses_In_Last_Week()
        {
            var client = new Mock<DurableTaskClient>(MockBehavior.Strict);
            client.Setup(c => c.PurgeInstancesAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<IReadOnlyCollection<OrchestrationRuntimeStatus>>(),
                    default))
                .ReturnsAsync(default(PurgeResult));

            var logger = new Mock<ILogger>().Object;
            var context = new TestFunctionContext(logger);
            var sut = new CleanupWorkflowsTimer();

            await sut.CleanupOldWorkflows(new object(), client.Object, context);

            client.Verify(c => c.PurgeInstancesAsync(
                It.Is<DateTimeOffset>(d => d < DateTimeOffset.UtcNow),
                It.Is<DateTimeOffset>(d => d <= DateTimeOffset.UtcNow),
                It.Is<IReadOnlyCollection<OrchestrationRuntimeStatus>>(s =>
                    s.SequenceEqual(new[]
                    {
                        OrchestrationRuntimeStatus.Completed,
                        OrchestrationRuntimeStatus.Canceled,
                        OrchestrationRuntimeStatus.ContinuedAsNew,
                        OrchestrationRuntimeStatus.Failed,
                        OrchestrationRuntimeStatus.Terminated
                    })),
                default), Times.Once);
        }
    }
}
