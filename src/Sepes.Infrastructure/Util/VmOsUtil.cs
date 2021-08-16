using Sepes.Common.Util;
using Sepes.Infrastructure.Model;

namespace Sepes.Infrastructure.Util
{
    public static class VmOsUtil
    {
        public static string GetOsName(CloudResource resource)
        {
            var vmSettings = CloudResourceConfigStringSerializer.VmSettings(resource.ConfigString);

            if (vmSettings != null)
            {
                if (string.IsNullOrWhiteSpace(vmSettings.OperatingSystemDisplayName))
                {
                    return vmSettings.OperatingSystemCategory;
                }

                return vmSettings.OperatingSystemDisplayName;
            }

            return null;
        }
    }
}