using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmploymentCheck.Data.Models;
using SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Activities;

namespace SFA.DAS.EmploymentCheck.Functions.AzureFunctions.Orchestrators
{
    public class CreateEmploymentCheckCacheRequestsOrchestrator
    {
        private readonly ILogger<CreateEmploymentCheckCacheRequestsOrchestrator> _logger;
        private readonly IEmploymentCheckDataValidator _employmentCheckDataValidator;

        public CreateEmploymentCheckCacheRequestsOrchestrator(
            ILogger<CreateEmploymentCheckCacheRequestsOrchestrator> logger,
            IEmploymentCheckDataValidator employmentCheckDataValidator)
        {
            _logger = logger;
            _employmentCheckDataValidator = employmentCheckDataValidator;
        }

        [Function(nameof(CreateEmploymentCheckCacheRequestsOrchestrator))]
        public async Task CreateEmploymentCheckRequestsTask([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            Data.Models.EmploymentCheck employmentCheck;
            do
            {
                employmentCheck = await context.CallActivityAsync<Data.Models.EmploymentCheck>(nameof(GetEmploymentCheckActivity), null);
                if (employmentCheck != null)
                {
                    var learnerNiNumber = await context.CallActivityAsync<LearnerNiNumber>(nameof(GetLearnerNiNumberActivity), employmentCheck);
                    var employerPayeSchemes = await context.CallActivityAsync<EmployerPayeSchemes>(nameof(GetEmployerPayeSchemesActivity), employmentCheck);

                    var data = new EmploymentCheckData(employmentCheck, learnerNiNumber, employerPayeSchemes);
                    var errors = _employmentCheckDataValidator.EmploymentCheckDataHasError(data);

                    if (string.IsNullOrEmpty(errors))
                    {
                        await context.CallActivityAsync(nameof(CreateEmploymentCheckCacheRequestActivity), data);
                    }
                    else
                    {
                        employmentCheck.ErrorType = errors;
                        await context.CallActivityAsync(nameof(StoreCompletedEmploymentCheckActivity), data);
                    }
                }
            }
            while (employmentCheck != null);
        }
    }
}
