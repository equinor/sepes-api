using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Common.Util;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class DapperModelServiceBase : ModelServiceBase
    {
        IStudyPermissionService _studyPermissionService;

        public DapperModelServiceBase(IConfiguration configuration, ILogger logger, IUserService userService, IStudyPermissionService studyPermissionService)
            : base(configuration, logger, userService)
        {
            _studyPermissionService = studyPermissionService;
        }

        protected string GetDbConnectionString()
        {
            var isIntegrationTest = ConfigUtil.GetBoolConfig(_configuration, ConfigConstants.IS_INTEGRATION_TEST);

            if (isIntegrationTest)
            {
                return _configuration[ConfigConstants.DB_INTEGRATION_TEST_CONNECTION_STRING];
            }

            return _configuration[ConfigConstants.DB_READ_WRITE_CONNECTION_STRING];
        }

        protected async Task<IEnumerable<T>> RunDapperQueryMultiple<T>(string query, object parameters = null)
        {
            using (var connection = new SqlConnection(GetDbConnectionString()))
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                return await connection.QueryAsync<T>(query, parameters);
            }
        }

        protected async Task<T> RunDapperQuerySingleAsync<T>(string query, object parameters = null) where T : SingleEntityDapperResult
        {
            using (var connection = new SqlConnection(GetDbConnectionString()))
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                return await connection.QuerySingleOrDefaultAsync<T>(query, parameters);
            }
        }

        protected string WrapSingleEntityQuery(UserDto currentUser, string dataQuery, UserOperation operation)
        {
            var accessWherePart = StudyAccessQueryBuilder.CreateAccessWhereClause(currentUser, operation);

            var completeQuery = $"WITH dataCte AS ({dataQuery})";
            completeQuery += " ,accessCte as (SELECT [Id] FROM Studies s INNER JOIN [dbo].[StudyParticipants] sp on s.Id = sp.StudyId WHERE s.Id=@studyId";

            if (!string.IsNullOrWhiteSpace(accessWherePart))
            {
                completeQuery += $" AND ({accessWherePart})";
            }

            completeQuery += " ) SELECT DISTINCT d.*, (CASE WHEN a.Id IS NOT NULL THEN 1 ELSE 0 END) As Authorized from dataCte d LEFT JOIN accessCte a on d.StudyId = a.Id ";

            return completeQuery;
        }

        protected async Task<T> RunSingleEntityQueryWithPermissionCheck<T>(UserDto currentUser, string dataQuery, UserOperation operation, object parameters = null) where T : SingleEntityDapperResult
        {
            var completeQuery = WrapSingleEntityQuery(currentUser, dataQuery, operation);
            var singleEntity = await RunDapperQuerySingleAsync<T>(completeQuery, parameters);

            _studyPermissionService.VerifyAccessOrThrow(singleEntity, currentUser, operation);

            return singleEntity;
        }
    }
}

