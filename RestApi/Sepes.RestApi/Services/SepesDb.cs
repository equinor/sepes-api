using System;
using System.Data.SqlClient;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Sepes.RestApi.Model;
//using Newtonsoft.Json.Linq;

namespace Sepes.RestApi.Services
{

    public class SepesDb : ISepesDb
    {
        private SqlConnection connection;
        private IConfiguration Configuration { get; set; }

        public SepesDb()
        {
            Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets<SepesDb>()
                .Build();

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

        /*public JObject getPodList(Pod input)
        {
            JObject json = new JObject();
            try
            {
                using (connection)
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT PodID, PodName, StudyID ");
                    sb.Append("FROM [dbo].[tblPod] ");
                    sb.Append("WHERE @studyID ");
                    sb.Append("LIKE StudyID ");
                    sb.Append("FOR JSON AUTO ");
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        command.Parameters.AddWithValue("@studyID", input.studyID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                JToken tokenObject = JToken.Parse(reader.GetString(0));
                                json.Add("pod", tokenObject);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            connection.Close();
            return json;
        }*/

        public int createStudy(Study study)
        {
            try
            {
                connection.Open();

                // insert study
                string sqlStudy = "INSERT INTO [dbo].[tblStudy] (StudyName) VALUES (@studyName) SELECT CAST(scope_identity() AS int)";

                SqlCommand command = new SqlCommand(sqlStudy, connection);
                command.Parameters.AddWithValue("@studyName", study.studyName);
                int studyId = (int)command.ExecuteScalar();
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
                return 0;
            }
            finally
            {
                connection.Close();
            }

            return 1;
        }

        /*public int createStudy(JObject study)
        {
            return createStudy(study.ToObject<Study>());
        }
        public int createPod(Pod pod)
        {
            try
            {
                connection.Open();

                // insert study
                string sqlStudy = "INSERT INTO [dbo].[tblPod] (StudyID, PodName) VALUES (@studyID , @podName) SELECT CAST(scope_identity() AS int)";
                //TODO add studyID
                SqlCommand command = new SqlCommand(sqlStudy, connection);
                command.Parameters.AddWithValue("@podName", pod.podName);
                command.Parameters.AddWithValue("@studyID", pod.studyID);
                //int podId = (int)command.ExecuteScalar(); Currently not used
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

        public int createUser(User user)
        {
            try
            {
                connection.Open();

                // insert user
                string sqlStudy = "INSERT INTO [dbo].[tblPod] (UserName, UserEmail, UserGroup) VALUES (@userName , @userEmail , @userGroup) SELECT CAST(scope_identity() AS int)";
                SqlCommand command = new SqlCommand(sqlStudy, connection);
                command.Parameters.AddWithValue("@userName", user.userName);
                command.Parameters.AddWithValue("@userEmail", user.userEmail);
                command.Parameters.AddWithValue("@userGroup", user.userGroup);
                //int userID = (int)command.ExecuteScalar(); Currently not used
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
        }*/

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

        /*//Search strings
        public string searchDatasetList(JObject search)
        {
            string data = "";
            using (connection)
            {
                connection.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * ");
                sb.Append("FROM [dbo].[tblDataset] ");
                sb.Append("WHERE DatasetName");
                sb.Append("LIKE @datasetName");
                sb.Append("FOR JSON AUTO ");
                string sql = sb.ToString();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@datasetName", "%" + search.GetValue("searchstring").ToString() + "%");
                using (command)
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data = reader.GetString(0);
                        }
                    }
                }
            }

            return data;
        }
        public string searchUserList(JObject search)
        {
            string data = "";
            using (connection)
            {
                connection.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * ");
                sb.Append("FROM [dbo].[tblUser] ");
                sb.Append("WHERE (");
                sb.Append("[UserEmail] LIKE @userName OR ");
                sb.Append("[UserName] LIKE @userName) ");
                sb.Append("FOR JSON AUTO ");
                string sql = sb.ToString();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@userName", "%" + search.GetValue("searchstring").ToString() + "%");
                using (command)
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data = reader.GetString(0);
                        }
                    }
                }
            }

            return data;
        }

        public string searchStudyList(JObject search)
        {
            string data = "";
            using (connection)
            {
                connection.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * ");
                sb.Append("FROM [dbo].[tblUser] ");
                sb.Append("WHERE StudyName");
                sb.Append("LIKE @studyName");
                sb.Append("FOR JSON AUTO ");
                string sql = sb.ToString();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@studyName", "%" + search.GetValue("searchstring").ToString() + "%");
                using (command)
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data = reader.GetString(0);
                        }
                    }
                }
            }

            return data;
        }*/

    }

}
