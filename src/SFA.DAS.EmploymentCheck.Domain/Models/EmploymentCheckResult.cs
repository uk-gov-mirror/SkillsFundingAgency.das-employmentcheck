using System;
using Newtonsoft.Json;

namespace SFA.DAS.EmploymentCheck.Domain.Models
{
    public class EmploymentCheckResult
    {
        [JsonProperty("empref")]
        public string empref { get; set; }

        [JsonProperty("nino")]
        public string nino { get; set; }

        [JsonProperty("fromDate")]
        public DateTime fromDate { get; set; }

        [JsonProperty("toDate")]
        public DateTime toDate { get; set; }

        [JsonProperty("employed")]
        public bool employed { get; set; }

    }
}
