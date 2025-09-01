using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using SFA.DAS.EmploymentCheck.Data.Models;
using SFA.DAS.EmploymentCheck.Queries;
using SFA.DAS.EmploymentCheck.Queries.GetNiNumber;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Activities
{
    public class GetLearnerNiNumberActivity
    {
        private readonly IQueryDispatcher _dispatcher;

        public GetLearnerNiNumberActivity(IQueryDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [Function(nameof(GetLearnerNiNumberActivity))]
        public async Task<LearnerNiNumber> Get([ActivityTrigger] Data.Models.EmploymentCheck employmentCheck)
        {
            var result = await _dispatcher.Send<GetNiNumberQueryRequest, GetNiNumberQueryResult>(new GetNiNumberQueryRequest(employmentCheck));
            return result.LearnerNiNumber;
        }
    }
}