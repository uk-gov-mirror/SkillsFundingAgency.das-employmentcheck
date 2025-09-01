using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using SFA.DAS.EmploymentCheck.Data.Models;
using SFA.DAS.EmploymentCheck.Queries;
using SFA.DAS.EmploymentCheck.Queries.ProcessEmploymentCheckCacheRequest;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Activities
{
    public class GetEmploymentCheckCacheRequestActivity
    {
        private readonly IQueryDispatcher _dispatcher;

        public GetEmploymentCheckCacheRequestActivity(IQueryDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [Function(nameof(GetEmploymentCheckCacheRequestActivity))]
        public async Task<EmploymentCheckCacheRequest[]> GetEmploymentCheckRequestActivityTask([ActivityTrigger] object input)
        {
            var result = await _dispatcher.Send<ProcessEmploymentCheckCacheRequestQueryRequest, ProcessEmploymentCheckCacheRequestQueryResult>(new ProcessEmploymentCheckCacheRequestQueryRequest());
            return result.EmploymentCheckCacheRequest;
        }
    }
}