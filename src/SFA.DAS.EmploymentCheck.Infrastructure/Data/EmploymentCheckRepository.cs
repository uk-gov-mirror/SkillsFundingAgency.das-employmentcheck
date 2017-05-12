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

        public async Task<T> GetEmploymentCheck<T>(string NINumber)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@niNumber", NINumber, DbType.String);


                return await c.QuerySingleOrDefaultAsync<T>(
                sql: "[DAS_GetEmploymentDetails]",
                param: parameters,
                commandType: CommandType.StoredProcedure);

            });
            return result;
        }

        public async Task StoreEmploymentCheck<T>(string paye, string nino, DateTime fromDate, DateTime toDate)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@paye", paye, DbType.String);
                parameters.Add("@nino", nino, DbType.String);
                parameters.Add("@fromDate", fromDate, DbType.Date);
                parameters.Add("@toDate", toDate, DbType.Date);

                return await c.ExecuteAsync(
                    sql: "[DAS_StoreEmploymentCheck]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
        }
    }
}
