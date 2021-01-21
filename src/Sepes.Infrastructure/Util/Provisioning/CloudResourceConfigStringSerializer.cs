using Newtonsoft.Json;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Dto.VirtualMachine;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Util
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

        public static List<CloudResourceDesiredRoleAssignmentDto> DesiredRoleAssignment(string settingAsString)
        {
            return DeserializeInternal<List<CloudResourceDesiredRoleAssignmentDto>>(settingAsString);
        }

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T DeserializeInternal<T>(string settings)
        {
            return JsonConvert.DeserializeObject<T>(settings);
        }
    }
}
