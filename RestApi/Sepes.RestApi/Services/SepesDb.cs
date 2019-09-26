using System;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Sepes.RestApi.Model;

namespace Sepes.RestApi.Services
{

public class SepesDb : ISepesDb
{
    private SqlConnection connection;
    private IConfiguration Configuration {get; set;}
    
    public SepesDb() {
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

    public JObject getDatasetList()
    {
        JObject json = new JObject();
        try {
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
                            JToken tokenObject = JToken.Parse(reader.GetString(0));
                            json.Add("dataset", tokenObject);
                        }
                    }
                }

                using (SqlCommand command = new SqlCommand(sqlUsers, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            JToken tokenObject = JToken.Parse(reader.GetString(0));
                            json.Add("users", tokenObject);
                        }
                    }
                }
            }
        } catch(SqlException ex) {
            Console.WriteLine(ex.ToString());
        }
        
        return json;
    }

    public int createStudy(Study study)
    {
        try {
            connection.Open();

            // insert study
            string sqlStudy = "INSERT INTO [dbo].[tblStudy] (StudyName) VALUES (@studyName) SELECT CAST(scope_identity() AS int)";

            SqlCommand command = new SqlCommand(sqlStudy, connection);
            command.Parameters.AddWithValue("@studyName", study.studyName);
            int studyId = (int)command.ExecuteScalar();

            Console.WriteLine("### SepesDB: StudyID "+studyId);

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
        catch (SqlException ex) {
            Console.WriteLine(ex.ToString());
            return 0;
        }
        finally {
            connection.Close();
        }

        return 1;
    }
    
    public int createStudy(JObject study)
    {
        return createStudy(study.ToObject<Study>());
    }

    private static void createInsertValues(int studyId, int[] array, StringBuilder strBuilder)
    {
        for (int i = 0; i < array.Length; i++)
        {
            strBuilder.Append("(" + array[i] + ", " + studyId + ")");
            if (i != array.Length-1)
            {
                strBuilder.Append(", ");
            }
        }

        Console.WriteLine(strBuilder.ToString());
    }

    }

}
