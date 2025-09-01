using AutoFixture;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Extensions;
using SFA.DAS.EmploymentCheck.Functions.UnitTests.TestHelpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmploymentCheck.Functions.UnitTests.AzureFunctions.Extensions
{
    public class DurableOrchestrationClientExtensionsTests
    {
        private string _orchestratorName;
        private Mock<DurableTaskClient> _starter;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _orchestratorName = _fixture.Create<string>();
            _starter = new Mock<DurableTaskClient>(MockBehavior.Strict);
        }

        private static async IAsyncEnumerable<OrchestrationMetadata> Empty() { yield break; }
        private static async IAsyncEnumerable<OrchestrationMetadata> Single() { yield return default; }

        [Test]
        public async Task Does_Not_Start_Orchestrator_When_Finds_Existing_Instances()
        {
            _starter.Setup(x => x.GetAllInstancesAsync(It.IsAny<OrchestrationQuery>()))
                    .Returns(new TestAsyncPageable<OrchestrationMetadata>(Single()));

            await _starter.Object.StartIfNotRunning(_orchestratorName);

            _starter.Verify(x => x.ScheduleNewOrchestrationInstanceAsync(
                It.IsAny<TaskName>(), It.IsAny<object>(), It.IsAny<StartOrchestrationOptions>(), default), Times.Never);
        }

        [Test]
        public async Task Starts_Orchestrator_When_No_Instances()
        {
            _starter.Setup(x => x.GetAllInstancesAsync(It.IsAny<OrchestrationQuery>()))
                    .Returns(new TestAsyncPageable<OrchestrationMetadata>(Empty()));

            _starter.Setup(x => x.ScheduleNewOrchestrationInstanceAsync(
                    It.IsAny<TaskName>(), It.IsAny<object>(), It.IsAny<StartOrchestrationOptions>(), default))
                .ReturnsAsync(_fixture.Create<string>());

            await _starter.Object.StartIfNotRunning(_orchestratorName);

            _starter.Verify(x => x.ScheduleNewOrchestrationInstanceAsync(
                It.Is<TaskName>(t => t.Name == _orchestratorName),
                null,
                It.IsAny<StartOrchestrationOptions>(),
                default), Times.Once);
        }
    }
}
