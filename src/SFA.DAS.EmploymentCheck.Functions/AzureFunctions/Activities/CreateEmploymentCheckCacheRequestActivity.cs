using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using SFA.DAS.EmploymentCheck.Abstractions;
using SFA.DAS.EmploymentCheck.Commands.CreateEmploymentCheckCacheRequest;
using SFA.DAS.EmploymentCheck.Data.Models;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Activities
{
    public class CreateEmploymentCheckCacheRequestActivity
    {
        private readonly ICommandDispatcher _dispatcher;

        public CreateEmploymentCheckCacheRequestActivity(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [Function(nameof(CreateEmploymentCheckCacheRequestActivity))]
        public async Task Create([ActivityTrigger] EmploymentCheckData employmentCheckData)
        {
            await _dispatcher.Send(new CreateEmploymentCheckCacheRequestCommand(employmentCheckData));
        }
    }
}