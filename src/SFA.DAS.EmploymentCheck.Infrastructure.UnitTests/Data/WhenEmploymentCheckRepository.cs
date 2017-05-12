using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmploymentCheck.Domain.Models;
using SFA.DAS.EmploymentCheck.Infrastructure.Data;
using System.Data.SqlClient;

namespace SFA.DAS.EmploymentCheck.Infrastructure.IntegrationTests.Data
{
    [TestFixture]
    public class WhenEmploymentCheckRepository
    {
        //TODO Move connections string to config
        string _connectionString = "Server=.;Database=SFA.DAS.EmploymentCheck;Trusted_Connection=True;";

        [SetUp]
        public void Arrange()
        {
            var cleanup = @"truncate table [dbo].[DAS_EmployeeDetails] 
                            truncate table [dbo].[DAS_EmploymentCheck] ";

            SqlCommand cmd = new SqlCommand(cleanup, new SqlConnection(_connectionString));
            cmd.Connection.Open();
            var result = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

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
