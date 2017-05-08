using MediatR;
using System.Threading.Tasks;
using SFA.DAS.EmploymentCheck.Domain.Models;
using SFA.DAS.EmploymentCheck.Application.Services;
using SFA.DAS.EmploymentCheck.Infrastructure.Data;

namespace SFA.DAS.EmploymentCheck.Application.Commands.CreateEmploymentDetails
{
    public class CreateEmploymentDetailsEventHandler: IAsyncNotificationHandler<CreateEmploymentDetailsCommand> 
    {
        EmploymentCheckRepository _repository;
        public CreateEmploymentDetailsEventHandler(EmploymentCheckRepository repository)
        {
            _repository  = repository;
        }
        
        public async Task Handle(CreateEmploymentDetailsCommand employee)
        {
            await _repository.StoreEmploymentDetails<HmrcEmploymentCheckRequest>(employee.Empref, employee.Nino, employee.FromDate, employee.ToDate);
        }
    }
}
