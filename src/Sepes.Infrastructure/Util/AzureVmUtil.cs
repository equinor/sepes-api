using Microsoft.Azure.Management.Compute.Models;
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

        public static string GetCategory(string vmName)
        {
            if (vmName.ToLower().Contains("standard_e"))
            {
                return "memory";
            }
            else if (vmName.ToLower().Contains("standard_nv"))
            {
                return "gpu";
            }
            else if (vmName.ToLower().Contains("standard_f"))
            {
                return "compute";
            }

            return "unknowncategory";
        }

        public static string GetDisplayTextForDropdown(VirtualMachineSize vmSizeInfo)
        {
            return $"{vmSizeInfo.Name} ({vmSizeInfo.NumberOfCores} cores, {vmSizeInfo.MemoryInMB} MB Memory, os disk: {vmSizeInfo.OsDiskSizeInMB}, max data disks: {vmSizeInfo.MaxDataDiskCount})";
        }
    }
}
