using Sepes.Common.Util;
using System.Collections.Generic;
using System.Linq;

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
            return JsonSerializerUtil.Deserialize<Dictionary<string, string>>(tags);
        }
        
        public static string TagDictionaryToString(Dictionary<string, string> tags)
        {
            return JsonSerializerUtil.Serialize(tags);
        }
    }
}