using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmploymentCheck.Domain.Models
{
    public class HmrcEmploymentCheckRequest
    {

        [JsonProperty("empref")]
        public string Empref { get; set; }

        [JsonProperty("nino")]
        public string Nino { get; set; }

        [JsonProperty("fromDate")]
        public DateTime FromDate { get; set; }

        [JsonProperty("empref")]
        public DateTime ToDate { get; set; }

    }
}
