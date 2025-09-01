using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using SFA.DAS.EmploymentCheck.Abstractions;
using SFA.DAS.EmploymentCheck.Commands.StoreCompletedEmploymentCheck;
using SFA.DAS.EmploymentCheck.Data.Models;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Activities
{
    public class StoreCompletedEmploymentCheckActivity
    {
        private readonly ICommandDispatcher _dispatcher;

        public StoreCompletedEmploymentCheckActivity(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [Function(nameof(StoreCompletedEmploymentCheckActivity))]
        public async Task Store([ActivityTrigger] EmploymentCheckData employmentCheckData)
        {
            await _dispatcher.Send(new StoreCompletedEmploymentCheckCommand(employmentCheckData.EmploymentCheck));
        }
    }
}