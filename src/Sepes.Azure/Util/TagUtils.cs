using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Sepes.Azure.Util
{
    public static class TagUtils
    {
        public static IDictionary<string, string> TagReadOnlyDictionaryToDictionary(IReadOnlyDictionary<string, string> tags)
        {
            return tags.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static Dictionary<string, string> TagStringToDictionary(string tags)
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(tags);
        }
        
        public static string TagDictionaryToString(Dictionary<string, string> tags)
        {
            return JsonSerializer.Serialize(tags);
        }
    }
}