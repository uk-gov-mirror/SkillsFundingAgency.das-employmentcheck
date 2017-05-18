using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmploymentCheck.Infrastructure.Data;

namespace SFA.DAS.EmploymentCheck.Application.Commands.FindUpdatedNINumbers
{
    class FindUpdatedNINumbersHandler : IAsyncNotificationHandler<FindUpdatedNINumbersCommand>
    {
        SubmissionsRespository _repository;

        public FindUpdatedNINumbersHandler(SubmissionsRespository repository)
        {
            _repository = repository;
        }

        public async Task Handle(FindUpdatedNINumbersCommand notification)
        {
            await _repository.StoreSubmissionEvents<Provider.Events.Api.Types.PageOfResults<Task>>(notification);
        }
    }
}
