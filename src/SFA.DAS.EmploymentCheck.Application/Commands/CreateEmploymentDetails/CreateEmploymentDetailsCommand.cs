using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmploymentCheck.Application.Commands.CreateEmploymentDetails
{
    public class CreateEmploymentDetailsCommand: IAsyncNotification 

    {
        public string EmploymentCheckId { get; set; }
    }
}
