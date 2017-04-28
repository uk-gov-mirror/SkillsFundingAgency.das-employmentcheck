using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmploymentCheck.Application.Configuration
{
    public class Configuration: IConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public HmrcConfiguration HmrcConfig { get; set; }
    }
}
