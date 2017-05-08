using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmploymentCheck.Domain.Models
{
    public class EmployeeDetails
    {
        public string Empref { get; set; }
        public string Nino { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
