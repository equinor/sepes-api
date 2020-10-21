using Sepes.Infrastructure.Model;

namespace Sepes.Infrastructure.Util
{
    public static class AzureVmUtil
    {
      public static string GetOsName(SandboxResource resource)
        {
            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(resource.ConfigString);

            if(vmSettings != null)
            {
                return vmSettings.Distro;
            }

            return null;
        }
    }
}
