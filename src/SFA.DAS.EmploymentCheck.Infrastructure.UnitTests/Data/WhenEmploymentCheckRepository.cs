using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmploymentCheck.Domain.Models;
using SFA.DAS.EmploymentCheck.Infrastructure.Data;
using System.Data.SqlClient;
using SFA.DAS.EmploymentCheck.Infrastructure.UnitTests.Data;

namespace SFA.DAS.EmploymentCheck.Infrastructure.IntegrationTests.Data
{
    [TestFixture]
    public class WhenEmploymentCheckRepository : BaseDataTest
    {

        [Test]
        public async Task ThenEmployeeDetailsAreStored()
        {
            var employee = new HmrcEmploymentCheckRequest();
            employee.Empref = "123/AB12345";
            employee.FromDate = new DateTime(2017, 05, 01);
            employee.ToDate = new DateTime(2018, 05, 01);
            employee.Nino = "AB123456A";
         
            var repo = new EmploymentCheckRepository(_connectionString);
            await repo.StoreEmploymentCheck<EmployeeDetails>(employee.Empref, employee.Nino, employee.FromDate, employee.ToDate);

            var recordid = await repo.GetEmploymentCheck<EmployeeDetails>(employee.Nino);

            Assert.That(recordid.NiNumber, Is.EqualTo(employee.Nino));
        }


    }
}
