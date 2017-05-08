using System;
using Newtonsoft.Json;

namespace SFA.DAS.EmploymentCheck.Domain.Models
{
    public class HmrcEmploymentCheckResponse
    {

        [JsonProperty("employed")]
        public bool Employed { get; set; }
    }
}
