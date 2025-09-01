using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Triggers;
using SFA.DAS.EmploymentCheck.Functions.UnitTests.TestHelpers;

namespace SFA.DAS.EmploymentCheck.Functions.UnitTests.AzureFunctions.Triggers.TriggerHelperTests
{
    public class WhenGettingRunningInstances
    {
        private static async IAsyncEnumerable<OrchestrationMetadata> Empty() { yield break; }
        private static async IAsyncEnumerable<OrchestrationMetadata> Single() { yield return default; }

        [Test]
        public async Task Returns_Null_When_None()
        {
            var helper = new TriggerHelper();
            var client = new Mock<DurableTaskClient>();
            client.Setup(c => c.GetAllInstancesAsync(It.IsAny<OrchestrationQuery>()))
                .Returns(new TestAsyncPageable<OrchestrationMetadata>(Empty()));

            var logger = new Mock<ILogger>().Object;
            var result = await helper.GetRunningInstances("name", "prefix", client.Object, logger);

            Assert.IsNull(result);
        }

        [Test]
        public async Task Returns_Metadata_When_Exists()
        {
            var helper = new TriggerHelper();
            var client = new Mock<DurableTaskClient>();
            client.Setup(c => c.GetAllInstancesAsync(It.IsAny<OrchestrationQuery>()))
                .Returns(new TestAsyncPageable<OrchestrationMetadata>(Single()));

            var logger = new Mock<ILogger>().Object;
            var result = await helper.GetRunningInstances("name", "prefix", client.Object, logger);

            Assert.IsNotNull(result);
        }
    }
}