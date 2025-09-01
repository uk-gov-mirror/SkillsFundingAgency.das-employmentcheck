using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using SFA.DAS.EmploymentCheck.Queries;
using SFA.DAS.EmploymentCheck.Queries.GetEmploymentCheck;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Activities
{
    public class GetEmploymentCheckActivity
    {
        private readonly IQueryDispatcher _dispatcher;

        public GetEmploymentCheckActivity(IQueryDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [Function(nameof(GetEmploymentCheckActivity))]
        public async Task<Data.Models.EmploymentCheck> Get([ActivityTrigger] object input)
        {
            _ = input;
            var result = await _dispatcher.Send<GetEmploymentCheckQueryRequest, GetEmploymentCheckQueryResult>(new GetEmploymentCheckQueryRequest());
            return result.EmploymentCheck;
        }
    }
}