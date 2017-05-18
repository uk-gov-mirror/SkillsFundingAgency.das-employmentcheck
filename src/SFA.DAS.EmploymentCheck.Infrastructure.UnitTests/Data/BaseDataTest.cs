using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Data.SqlClient;

namespace SFA.DAS.EmploymentCheck.Infrastructure.UnitTests.Data
{
    [TestFixture]
    public abstract class BaseDataTest
    {
        //TODO Move connections string to config
        protected string _connectionString = "Server=.;Database=SFA.DAS.EmploymentCheck;Trusted_Connection=True;";

        [SetUp]
        public void Arrange()
        {
            var cleanup = @"truncate table [dbo].[DAS_EmployeeDetails] 
                            truncate table [dbo].[DAS_EmploymentCheck]
                            truncate table [dbo].[DAS_SubmissionEvents] ";

            SqlCommand cmd = new SqlCommand(cleanup, new SqlConnection(_connectionString));
            cmd.Connection.Open();
            var result = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
    }
}
