using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using SFA.DAS.EmploymentCheck.Queries;
using SFA.DAS.EmploymentCheck.Queries.GetResponseEmploymentCheck;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Activities
{
    public class GetResponseEmploymentCheckActivity
    {
        private readonly IQueryDispatcher _dispatcher;

        public GetResponseEmploymentCheckActivity(IQueryDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [Function(nameof(GetResponseEmploymentCheckActivity))]
        public async Task<Data.Models.EmploymentCheck> Get([ActivityTrigger] object input)
        {
            var result = await _dispatcher.Send<GetResponseEmploymentCheckQueryRequest, GetResponseEmploymentCheckQueryResult>(new GetResponseEmploymentCheckQueryRequest());
            return result.EmploymentCheck;
        }
    }
}