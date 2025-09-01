using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using SFA.DAS.EmploymentCheck.Abstractions;
using SFA.DAS.EmploymentCheck.Commands.AbandonRelatedRequests;
using SFA.DAS.EmploymentCheck.Data.Models;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Activities
{
    public class AbandonRelatedRequestsActivity
    {
        private readonly ICommandDispatcher _dispatcher;

        public AbandonRelatedRequestsActivity(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [Function(nameof(AbandonRelatedRequestsActivity))]
        public async Task Create([ActivityTrigger] EmploymentCheckCacheRequest[] employmentCheckCacheRequests)
        {
            await _dispatcher.Send(new AbandonRelatedRequestsCommand(employmentCheckCacheRequests));
        }
    }
}