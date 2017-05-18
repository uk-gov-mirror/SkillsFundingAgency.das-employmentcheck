using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Api.Client;
using SFA.DAS.Provider.Events.Api.Types;
using System.Data;
using Dapper;

namespace SFA.DAS.EmploymentCheck.Infrastructure.Data
{
    public class SubmissionsRespository: BaseRepository
    {
        private readonly string _connectionString;

        public SubmissionsRespository(string connectionString): base(connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task StoreSubmissionEvents<T>(PageOfResults<SubmissionEvent> submissions)
        {
            foreach (SubmissionEvent e in submissions.Items)
            {
                await WithConnection(async c =>
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", e.Id, DbType.Int32);
                    parameters.Add("@AcademicYear", e.AcademicYear, DbType.String);
                    parameters.Add("@ActualEndDate", e.ActualEndDate, DbType.Date);
                    parameters.Add("@ActualStartDate", e.ActualStartDate, DbType.Date);
                    parameters.Add("@ApprenticeshipId", e.ApprenticeshipId, DbType.Int32);
                    parameters.Add("@EmployerReferenceNumber", e.EmployerReferenceNumber, DbType.Int32);
                    parameters.Add("@IlrFileName", e.IlrFileName, DbType.String);
                    parameters.Add("@NiNumber", e.NiNumber, DbType.String);
                    parameters.Add("@SubmittedDateTime", e.SubmittedDateTime, DbType.Date);
                    parameters.Add("@Uln", e.Uln, DbType.String);

                    return await c.ExecuteAsync(
                        sql: "[DAS_StoreSubmissionEvents]",
                        param: parameters,
                        commandType: CommandType.StoredProcedure);
                });
            }
        }

        public async Task<T> GetSubmissionEvent<T>(long eventId)
        {
            var result = await WithConnection(async c =>
            {


                var parameters = new DynamicParameters();
                parameters.Add("@Id", eventId, DbType.Int64);


                return await c.QuerySingleOrDefaultAsync<T>(
                sql: "[DAS_GetSubmissionEventById]",
                param: parameters,
                commandType: CommandType.StoredProcedure);

            });

            return result;
        }

    }
}
