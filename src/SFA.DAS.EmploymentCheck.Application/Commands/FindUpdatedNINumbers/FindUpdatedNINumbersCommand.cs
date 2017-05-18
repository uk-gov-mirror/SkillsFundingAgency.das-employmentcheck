using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.EmploymentCheck.Application.Commands.FindUpdatedNINumbers
{
    public class FindUpdatedNINumbersCommand : PageOfResults<SubmissionEvent>, IAsyncNotification
    {

        
        
        //public string AcademicYear { get; set; }

        //public string ActualEndDate { get; set; }
        //public string ActualStartDate { get; set; }
        //public string ApprenticeshipId { get; set; }
        //public string EmployerReferenceNumber { get; set; }
        //public string IlrFileName { get; set; }
        //public string NiNumber { get; set; }
        //public string SubmittedDateTime { get; set; }
        //public string Uln { get; set; }


    }
}
