using FluentAssertions;
using NUnit.Framework;
using System.Text.Json;

namespace SFA.DAS.EmploymentCheck.Queries.UnitTests.Dispatcher
{
    public class WhenQueryDispatcherExceptionIsCreated
    {
        [Test]
        public void Then_Create_QueryDispatcherException_No_Parameters()
        {
            // Arrange and Act
            var queryDispatcherException = new QueryDispatcherException();

            // Assert
            queryDispatcherException.Should().NotBeNull();
        }

        [Test]
        public void Then_Create_QueryDispatcherException_Using_Serialization()
        {
            // Arrange
            var expectedMessage = "ExceptionTest Message";
            var queryDispatcherException = new QueryDispatcherException(expectedMessage);
            QueryDispatcherException actual;

            // Act
            var json = JsonSerializer.Serialize(queryDispatcherException);
            actual = JsonSerializer.Deserialize<QueryDispatcherException>(json);

            // Assert
            actual.Should().NotBeNull();
            actual.Message.Should().Be(expectedMessage);
        }
    }
}