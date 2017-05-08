using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmploymentCheck.Domain.Models;
using SFA.DAS.EmploymentCheck.Infrastructure.Data;

namespace SFA.DAS.EmploymentCheck.Infrastructure.IntegrationTests.Data
{
    [TestFixture]
    public class WhenEmploymentCheckRepository
    {
        [Test]
        public async Task ThenEmployeeDetailsAreStored()
        {
            var employee = new HmrcEmploymentCheckRequest();
            employee.Empref = "123/AB12345";
            employee.FromDate = new DateTime(2017, 05, 01);
            employee.Nino = "AB123456A";

            //TODO Move connections string to config
            var repo = new EmploymentCheckRepository("Server=.;Database=SFA.DAS.EmploymentCheck;Trusted_Connection=True;");
            var result = repo.StoreEmploymentDetails<EmployeeDetails>(employee.Empref, employee.Nino, employee.FromDate, employee.ToDate);


            var recordid = await repo.GetEmploymentDetails<EmployeeDetails>(employee.Nino);
            Assert.Equals(recordid.Nino, employee.Nino);
        }
    }
}
