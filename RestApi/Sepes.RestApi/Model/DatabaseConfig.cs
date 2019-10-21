namespace Sepes.RestApi.Model
{

    public class DatabaseConfig
    {
        public string connectionString {get;}

        public DatabaseConfig(string connectionString){
            this.connectionString = connectionString;
        }
    }
}