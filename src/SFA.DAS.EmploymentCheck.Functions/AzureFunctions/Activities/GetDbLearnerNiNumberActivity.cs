using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using SFA.DAS.EmploymentCheck.Data.Models;
using SFA.DAS.EmploymentCheck.Queries;
using SFA.DAS.EmploymentCheck.Queries.GetDbNiNumber;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Activities
{
    public class GetDbLearnerNiNumberActivity
    {
        private readonly IQueryDispatcher _dispatcher;

        public GetDbLearnerNiNumberActivity(IQueryDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [Function(nameof(GetDbLearnerNiNumberActivity))]
        public async Task<LearnerNiNumber> Get([ActivityTrigger] Data.Models.EmploymentCheck employmentCheck)
        {
            var result = await _dispatcher.Send<GetDbNiNumberQueryRequest, GetDbNiNumberQueryResult>(new GetDbNiNumberQueryRequest(employmentCheck));
            return result.LearnerNiNumber;
        }
    }
}