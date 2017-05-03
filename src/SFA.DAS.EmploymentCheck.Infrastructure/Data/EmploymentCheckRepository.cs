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

        public async Task<T> GetEmploymentCheck<T>(int NINumber)
        {
            throw new NotImplementedException();

            //var result = await WithConnection(async c =>
            //{
            //    //var parameters = new DynamicParameters();
            //    //parameters.Add("@eventFeed", eventFeed, DbType.String);


            //        return await c.QuerySingleOrDefaultAsync<T>(
            //        sql: "[Data_Load].[GetLastProcessedEventId]",
            //        param: parameters,
            //        commandType: CommandType.StoredProcedure);

            //});
            //return result;
        }

        public async Task StoreEmploymentCheck<T>(string eventFeed, T id)
        {
            throw new NotImplementedException();

            //await WithConnection(async c =>
            //{
            //    var parameters = new DynamicParameters();
            //    parameters.Add("@eventFeed", eventFeed, DbType.String);
            //    parameters.Add("@lastProcessedEventId", id, DbType.String);

            //    return await c.ExecuteAsync(
            //        sql: "[Data_Load].[StoreLastProcessedEventId]",
            //        param: parameters,
            //        commandType: CommandType.StoredProcedure);
            //});
        }
    }
}
