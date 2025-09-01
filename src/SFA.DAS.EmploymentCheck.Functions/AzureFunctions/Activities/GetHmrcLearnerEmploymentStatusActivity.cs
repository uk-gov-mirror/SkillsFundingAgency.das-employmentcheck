using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using SFA.DAS.EmploymentCheck.Data.Models;
using SFA.DAS.EmploymentCheck.Queries;
using SFA.DAS.EmploymentCheck.Queries.GetHmrcLearnerEmploymentStatus;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Activities
{
    public class GetHmrcLearnerEmploymentStatusActivity
    {
        private readonly IQueryDispatcher _dispatcher;

        public GetHmrcLearnerEmploymentStatusActivity(IQueryDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [Function(nameof(GetHmrcLearnerEmploymentStatusActivity))]
        public async Task<EmploymentCheckCacheRequest> GetHmrcLearnerEmploymentStatusTask([ActivityTrigger] EmploymentCheckCacheRequest request)
        {
            var result = await _dispatcher.Send<GetHmrcLearnerEmploymentStatusQueryRequest, GetHmrcLearnerEmploymentStatusQueryResult>(new GetHmrcLearnerEmploymentStatusQueryRequest(request));
            return result.EmploymentCheckCacheRequest;
        }
    }
}