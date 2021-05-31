using System.Text.Json;

namespace Sepes.Common.Util
{
    public static class JsonSerializerUtil
    {
        public static T Deserialize<T>(string whatToDeserialize)
        {
            return JsonSerializer.Deserialize<T>(whatToDeserialize, GetDefaultOptions());
        }       

        public static string Serialize<T>(T whatToSerialize)
        {
            return JsonSerializer.Serialize<T>(whatToSerialize, GetDefaultOptions());
        }

        public static JsonSerializerOptions GetOptions(JsonNamingPolicy jsonNamingPolicy)
        {
            return new JsonSerializerOptions
            {   
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = jsonNamingPolicy,               
                IgnoreNullValues = true,
                AllowTrailingCommas = true              
            };
        }

        public static JsonSerializerOptions GetDefaultOptions()
        {
            return GetOptions(JsonNamingPolicy.CamelCase);
        }
    }
}
