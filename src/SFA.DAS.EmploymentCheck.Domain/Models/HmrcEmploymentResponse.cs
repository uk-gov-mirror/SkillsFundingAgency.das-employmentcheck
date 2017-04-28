using System;
using Newtonsoft.Json;

namespace SFA.DAS.EmploymentCheck.Domain.Models
{
    public class HmrcEmploymentResponse
    {
        [JsonProperty("empref")]
        public string Empref { get; set; }

        [JsonProperty("nino")]
        public string Nino { get; set; }

        [JsonProperty("fromDate")]
        public DateTime FromDate { get; set; }

        [JsonProperty("empref")]
        public DateTime ToDate { get; set; }

        [JsonProperty("employed")]
        public bool Employed { get; set; }
    }
}
