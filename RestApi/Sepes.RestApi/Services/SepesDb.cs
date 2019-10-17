using System;
using System.Data.SqlClient;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Sepes.RestApi.Model;
using System.Threading.Tasks;

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

        public string getDatasetList()
        {
            //JObject json = new JObject();
            string datasetstring = "";
            string userstring = "";
            try
            {
                using (connection)
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT DatasetId, DatasetName ");
                    sb.Append("FROM [dbo].[tblDataset] ");
                    sb.Append("FOR JSON AUTO ");
                    string sqlDataset = sb.ToString();

                    string sqlUsers = "SELECT UserId, UserName, UserEmail, UserGroup FROM dbo.tblUser FOR JSON AUTO";

                    using (SqlCommand command = new SqlCommand(sqlDataset, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //JToken tokenObject = JToken.Parse(reader.GetString(0)); //Parse is serialising into json
                                //json.Add("dataset", tokenObject); //Adds a top level header to the json
                                datasetstring = reader.GetString(0);
                                datasetstring.Insert(1, "{\"dataset\":");
                                datasetstring.Insert(datasetstring.Length - 2,"]");
                            }
                        }
                    }

                    using (SqlCommand command = new SqlCommand(sqlUsers, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //JToken tokenObject = JToken.Parse(reader.GetString(0)); //Parse is serialising into json
                                //json.Add("users", tokenObject);
                                userstring = reader.GetString(0);
                                userstring.Insert(1, "{\"dataset\":");
                                userstring.Insert(userstring.Length - 2,"]");
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            string response = datasetstring + "," + userstring;
            return response;
        }
        public int createStudy(Study study)
        {
            int studyId = -1;
            try
            {
                connection.Open();

                // insert study
                string sqlStudy = "INSERT INTO [dbo].[tblStudy] (StudyName) VALUES (@studyName) SELECT CAST(scope_identity() AS int)";

                SqlCommand command = new SqlCommand(sqlStudy, connection);
                command.Parameters.AddWithValue("@studyName", study.studyName);
                studyId = (int)command.ExecuteScalar();
                Console.WriteLine("### SepesDB: StudyID " + studyId);

                // insert user2study
                StringBuilder user2StudyBuilder = new StringBuilder();
                user2StudyBuilder.Append("INSERT INTO [dbo].[lnkUser2Study] (UserID, StudyID) VALUES ");
                createInsertValues(studyId, study.userIds, user2StudyBuilder);
                string sqlUser2Study = user2StudyBuilder.ToString();

                command = new SqlCommand(sqlUser2Study, connection);
                command.ExecuteNonQuery();

                // insert study2dataset
                StringBuilder study2datasetBuilder = new StringBuilder();
                study2datasetBuilder.Append("INSERT INTO [dbo].[lnkStudy2Dataset] (DatasetID, StudyID) VALUES ");
                createInsertValues(studyId, study.datasetIds, study2datasetBuilder);
                string sqlStudy2dataset = study2datasetBuilder.ToString();

                command = new SqlCommand(sqlStudy2dataset, connection);
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
            finally
            {
                connection.Close();
            }

            return studyId;
        }
        public async Task<Pod> createPod(string name, int studyId)
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
        public int updateStudy(Study study)
        {
            try
            {
                connection.Open();

                // insert study
                string sqlStudy = $"UPDATE [dbo].[tblStudy] SET Archived = '{study.archived}' WHERE StudyID = {study.studyId}";

                SqlCommand command = new SqlCommand(sqlStudy, connection);
                //command.Parameters.AddWithValue("@archived", study.archived);
                int studyNum = (int)command.ExecuteScalar();

                Console.WriteLine($"### SepesDB: Updated Study {studyNum} with archived = {study.archived}");
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
            finally
            {
                connection.Close();
            }

            return 1;
        }

        public string getStudies(bool archived)
        {
            string response = "";
            try
            {
                connection.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT StudyId, StudyName ");
                sb.Append("FROM [dbo].[tblStudy] ");
                sb.Append($"WHERE Archived {(archived ? "= 'True'" : "= 'False' OR Archived IS NULL")} ");
                sb.Append("FOR JSON AUTO ");
                string sqlStudies = sb.ToString();

                using (SqlCommand command = new SqlCommand(sqlStudies, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            response = reader.GetString(0);
                        }
                    }
                }
            }
            catch (SqlException ex) 
            {
                Console.WriteLine(ex.ToString());
            }
            finally 
            {
                connection.Close();
            }

            return response;
        }

    }

}
