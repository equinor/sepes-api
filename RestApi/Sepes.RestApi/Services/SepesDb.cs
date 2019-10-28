using System;
using System.Data.SqlClient;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Sepes.RestApi.Model;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sepes.RestApi.Services
{

    public class SepesDb : ISepesDb
    {
        private SqlConnection connection;

        public SepesDb(IConfiguration Configuration)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = Configuration["Sepes:sql:server"];
            builder.UserID = Configuration["Sepes:sql:user"];
            builder.Password = Configuration["Sepes:sql:password"];
            builder.InitialCatalog = Configuration["Sepes:sql:database"];

            connection = new SqlConnection(builder.ConnectionString);
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
        public async Task<int> createStudy(string studyName, int[] userIds, int[] datasetIds)
        {
            int studyId = -1;
            await connection.OpenAsync();
            try
            {
                //List<Task> tasks = new List<Task>();

                // insert study
                string sqlStudy = "INSERT INTO [dbo].[tblStudy] (StudyName) VALUES (@studyName) SELECT CAST(scope_identity() AS int)";

                SqlCommand command = new SqlCommand(sqlStudy, connection);
                command.Parameters.AddWithValue("@studyName", studyName);
                studyId = Convert.ToUInt16(await command.ExecuteScalarAsync());
                Console.WriteLine("### SepesDB: StudyID " + studyId);

                /* 
                // insert user2study
                StringBuilder user2StudyBuilder = new StringBuilder();
                user2StudyBuilder.Append("INSERT INTO [dbo].[lnkUser2Study] (UserID, StudyID) VALUES ");
                createInsertValues(studyId, userIds, user2StudyBuilder);
                string sqlUser2Study = user2StudyBuilder.ToString();

                command = new SqlCommand(sqlUser2Study, connection);
                await command.ExecuteNonQueryAsync();*/

                if (datasetIds.Length > 0)
                {
                    // insert study2dataset
                    StringBuilder study2datasetBuilder = new StringBuilder();
                    study2datasetBuilder.Append("INSERT INTO [dbo].[lnkStudy2Dataset] (DatasetID, StudyID) VALUES ");
                    createInsertValues(studyId, datasetIds, study2datasetBuilder);
                    string sqlStudy2dataset = study2datasetBuilder.ToString();

                    command = new SqlCommand(sqlStudy2dataset, connection);
                    await command.ExecuteNonQueryAsync();
                }
                
                //Task.WaitAll(tasks.ToArray()); //Might work, might not work
            }
            finally
            {
                await connection.CloseAsync();
            }

            return studyId;
        }

        public async Task<Pod> createPod(string name, int studyId) ////This is the one to steal from
        {
            await connection.OpenAsync();
            try
            {
                string sqlPod = "INSERT INTO [dbo].[tblPod] (StudyID, PodName) VALUES (@studyID , @podName) SELECT CAST(scope_identity() AS int)";
                
                SqlCommand command = new SqlCommand(sqlPod, connection);
                command.Parameters.AddWithValue("@podName", name);
                command.Parameters.AddWithValue("@studyID", studyId);
                var id = Convert.ToUInt16(await command.ExecuteScalarAsync());
                return new Pod(id, name, studyId);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        private static void createInsertValues(int studyId, int[] array, StringBuilder strBuilder)
        {
            for (int i = 0; i < array.Length; i++)
            {
                strBuilder.Append("(" + array[i] + ", " + studyId + ")");
                if (i != array.Length - 1)
                {
                    strBuilder.Append(", ");
                }
            }

            Console.WriteLine(strBuilder.ToString());
        }

        public async Task<int> updateStudy(int studyId, bool archived)
        {
            await connection.OpenAsync();
            try
            {
                string sqlStudy = $"UPDATE [dbo].[tblStudy] SET Archived = '{archived}' WHERE StudyID = {studyId}";
                SqlCommand command = new SqlCommand(sqlStudy, connection);
                Console.WriteLine($"### SepesDB: Updating Study {studyId} with archived = {archived}");
                return command.ExecuteNonQuery();
            }
            catch
            {
                Console.WriteLine($"### SepesDB: Updating Study {studyId} with archived = {archived} failed");
                return 0;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<string> getStudies(bool archived)
        {
            string response = "";
            await connection.OpenAsync();
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT StudyId, StudyName, Archived ");
                sb.Append("FROM [dbo].[tblStudy] ");
                sb.Append($"WHERE Archived {(archived ? "= 'True'" : "= 'False' OR Archived IS NULL")} ");
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

        public async Task<string> getPods(int studyId)
        {
            string response = "";
            await connection.OpenAsync();
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT StudyID, PodID, PodName ");
                sb.Append("FROM [dbo].[tblPod] ");
                sb.Append($"WHERE StudyID = {studyId} ");
                sb.Append("FOR JSON AUTO ");
                string sqlStudies = sb.ToString();

                using (SqlCommand command = new SqlCommand(sqlStudies, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
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

            Console.WriteLine("### getPods " + studyId);
            Console.WriteLine(response);
            return response;
        }
    }

}
