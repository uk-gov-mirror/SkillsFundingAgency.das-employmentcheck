using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmploymentCheck.Infrastructure.Data
{
    public class EmploymentCheckRepository: BaseRepository 
    {
        public EmploymentCheckRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<T> GetEmploymentDetails<T>(string NINumber)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@niNumber", NINumber, DbType.String);


                return await c.QuerySingleOrDefaultAsync<T>(
                sql: "[SFA.DAS.EmploymentCheck].[DAS_EmploymentDetails]",
                param: parameters,
                commandType: CommandType.StoredProcedure);

            });
            return result;
        }

        public async Task StoreEmploymentDetails<T>(string paye, string nino, DateTime fromDate, DateTime toDate)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@paye", paye, DbType.String);
                parameters.Add("@nino", nino, DbType.String);
                parameters.Add("@fromDate", nino, DbType.String);
                parameters.Add("@toDate", nino, DbType.String);

                return await c.ExecuteAsync(
                    sql: "[SFA.DAS.EmploymentCheck].[DAS_EmploymentDetails]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
        }
    }
}
