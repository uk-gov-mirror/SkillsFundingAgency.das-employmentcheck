using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using SFA.DAS.EmploymentCheck.Abstractions;
using SFA.DAS.EmploymentCheck.Commands.PublishEmploymentCheckResult;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Activities
{
    public class OutputEmploymentCheckResultsActivity
    {
        private readonly ICommandDispatcher _dispatcher;

        public OutputEmploymentCheckResultsActivity(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [Function(nameof(OutputEmploymentCheckResultsActivity))]
        public async Task Send([ActivityTrigger] Data.Models.EmploymentCheck employmentCheck)
        {
            await _dispatcher.Send(new PublishEmploymentCheckResultCommand(employmentCheck));
        }
    }
}