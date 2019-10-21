using Microsoft.Extensions.Configuration;
using Sepes.RestApi.Model;

namespace Sepes.RestApi.Services
{
    public interface IConfigService {
        DatabaseConfig databaseConfig {get;}
    }

    public class ConfigService: IConfigService {
        public DatabaseConfig databaseConfig {get;}

        public ConfigService(IConfiguration asp, IConfiguration sepes){
            databaseConfig = new DatabaseConfig(sepes["MSSQL_CONNECTION_STRING"]);
        }
    }
}