using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using SFA.DAS.EmploymentCheck.Data.Models;
using SFA.DAS.EmploymentCheck.Queries;
using SFA.DAS.EmploymentCheck.Queries.GetPayeSchemes;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Activities
{
    public class GetEmployerPayeSchemesActivity
    {
        private readonly IQueryDispatcher _dispatcher;

        public GetEmployerPayeSchemesActivity(IQueryDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [Function(nameof(GetEmployerPayeSchemesActivity))]
        public async Task<EmployerPayeSchemes> Get([ActivityTrigger] Data.Models.EmploymentCheck employmentCheck)
        {
            Guard.Against.Null(employmentCheck, nameof(employmentCheck));
            var result = await _dispatcher.Send<GetPayeSchemesQueryRequest, GetPayeSchemesQueryResult>(new GetPayeSchemesQueryRequest(employmentCheck));
            return result.EmployersPayeSchemes;
        }
    }
}