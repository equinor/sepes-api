using Newtonsoft.Json;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Dto.VirtualMachine;

namespace Sepes.Infrastructure.Util
{
    public static class SandboxResourceConfigStringSerializer
    {    

        public static VmSettingsDto VmSettings(string settingAsString)
        {
            return DeserializeInternal<VmSettingsDto>(settingAsString);
        }

        public static NetworkSettingsDto NetworkSettings(string settingAsString)
        {
            return DeserializeInternal<NetworkSettingsDto>(settingAsString);
        }

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        static T DeserializeInternal<T>(string settings)
        {
            return JsonConvert.DeserializeObject<T>(settings);
        }
    }
}
