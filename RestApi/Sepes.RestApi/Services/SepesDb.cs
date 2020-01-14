using System;
using System.Data.SqlClient;
using System.Text;
using Sepes.RestApi.Model;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace Sepes.RestApi.Services
{
    [ExcludeFromCodeCoverage]
    public class SepesDb : ISepesDb
    {
        private SqlConnection connection;

        public SepesDb(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public async Task<string> getDatasetList()
        {
            string response = "";
            await connection.OpenAsync();
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT DatasetId, DatasetName ");
                sb.Append("FROM [dbo].[tblDataset] ");
                sb.Append("FOR JSON AUTO ");
                string sqlStudies = sb.ToString();

                using (SqlCommand command = new SqlCommand(sqlStudies, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            response = reader.GetString(0);
                        }
                    }
                }
            }
            finally 
            {
                await connection.CloseAsync();
            }

            return response;
        }

        public async Task<Study> NewStudy(Study study)
        {
            Study saveStudy = study;
            await connection.OpenAsync();
            try
            {
                string jsonData = JsonSerializer.Serialize<Study>(study);
                string sqlNewStudy = "INSERT INTO dbo.Studies (JsonData) VALUES (@JsonData) SELECT CAST(scope_identity() AS int)";
                
                SqlCommand command = new SqlCommand(sqlNewStudy, connection);
                command.Parameters.AddWithValue("@JsonData", jsonData);
                int studyId = Convert.ToUInt16(await command.ExecuteScalarAsync());

                saveStudy = new Study(study.studyName, studyId, study.pods, study.sponsors, study.suppliers, 
                                        study.datasets, study.archived, study.userIds, study.datasetIds);
            }
            finally
            {
                await connection.CloseAsync();
            }
            await UpdateStudy(saveStudy);

            return saveStudy;
        }

        public async Task<bool> UpdateStudy(Study study)
        {
            await connection.OpenAsync();
            try
            {
                string jsonData = JsonSerializer.Serialize(study);
                string sqlUpdateStudy = $"UPDATE dbo.Studies SET JsonData = (@JsonData) WHERE StudyID = {study.studyId}";
                SqlCommand updateCommand = new SqlCommand(sqlUpdateStudy, connection);
                updateCommand.Parameters.AddWithValue("@JsonData", jsonData);
                await updateCommand.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<IEnumerable<Study>> GetAllStudies()
        {
            var studiesDB = new HashSet<StudyDB>();

            await connection.OpenAsync();
            try
            {
                string sqlStudies = "SELECT StudyId, JsonData FROM dbo.Studies";

                using (SqlCommand command = new SqlCommand(sqlStudies, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            try {
                                var study = JsonSerializer.Deserialize<StudyDB>(reader.GetString(1));
                                studiesDB.Add(study);
                            }
                            catch {}
                        }
                    }
                }
            }
            finally 
            {
                await connection.CloseAsync();
            }

            return studiesDB.Select(study => study.ToStudy());
        }

    }
}
