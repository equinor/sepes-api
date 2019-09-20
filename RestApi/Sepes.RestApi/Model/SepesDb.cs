using System;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Sepes.RestApi.Model
{

public class SepesDb
{
    private SqlConnection connection;
    public IConfiguration Configuration {get; set;}
    
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

    public string getDatasetList()
    {
        string data = "";
        using (connection)
        {
            connection.Open();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * ");
            sb.Append("FROM [dbo].[tblDataset] ");
            sb.Append("FOR JSON AUTO ");
            string sql = sb.ToString();

            using (SqlCommand command = new SqlCommand(sql, connection))
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

    public int createStudy(JObject study)
    {
        //JObject json = JObject.Parse(study);
        int returnValue = 0;

        try
        {
            connection.Open();
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO [dbo].[tblStudy] (StudyName) ");
            sb.Append("Values (@studyName) ");
            string sql = sb.ToString();

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@studyName", study.GetValue("studyName").ToString());
            returnValue = command.ExecuteNonQuery();
        }
        catch (System.Data.SqlClient.SqlException ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            connection.Close();
        }

        return returnValue;
    }
}

}
