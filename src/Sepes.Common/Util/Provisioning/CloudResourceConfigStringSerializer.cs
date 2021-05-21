using Sepes.Common.Dto;
using Sepes.Common.Dto.Sandbox;
using Sepes.Common.Dto.VirtualMachine;
using System.Collections.Generic;
using System.Text.Json;

namespace Sepes.Common.Util
{
    public static class CloudResourceConfigStringSerializer
    {    

        public static VmSettingsDto VmSettings(string settingAsString)
        {
            return DeserializeInternal<VmSettingsDto>(settingAsString);
        }

        public static NetworkSettingsDto NetworkSettings(string settingAsString)
        {
            return DeserializeInternal<NetworkSettingsDto>(settingAsString);
        }       

        public static CloudResourceOperationStateForRoleUpdate DesiredRoleAssignment(string settingAsString)
        {
            return DeserializeInternal<CloudResourceOperationStateForRoleUpdate>(settingAsString);
        }

        public static List<FirewallRule> DesiredFirewallRules(string settingAsString)
        {
            return DeserializeInternal<List<FirewallRule>>(settingAsString);
        }

        public static List<CorsRule> DesiredCorsRules(string settingAsString)
        {
            return DeserializeInternal<List<CorsRule>>(settingAsString);
        }

        public static string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public static T DeserializeInternal<T>(string settings)
        {
            return JsonSerializer.Deserialize<T>(settings);
        }
    }
}
