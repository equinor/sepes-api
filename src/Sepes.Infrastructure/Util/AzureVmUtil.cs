using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Models;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sepes.Infrastructure.Util
{
    public static class AzureVmUtil
    {
        public static string GetOsName(SandboxResource resource)
        {
            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(resource.ConfigString);

            if (vmSettings != null)
            {
                return vmSettings.OperatingSystem;
            }

            return null;
        }

        public static string GetSizeCategory(string vmName)
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

        public static string GetDisplayTextSizeForDropdown(VirtualMachineSize vmSizeInfo)
        {
            return $"{vmSizeInfo.Name} ({vmSizeInfo.NumberOfCores} cores, {vmSizeInfo.MemoryInMB} MB Memory, os disk: {vmSizeInfo.OsDiskSizeInMB}, max data disks: {vmSizeInfo.MaxDataDiskCount})";
        }

        public static string GetOsCategory(List<VmOsDto> osList, string operatingSystemName)
        {
            var foundOs = osList.Where(os => os.Key == operatingSystemName).FirstOrDefault();

            if (foundOs == null)
            {
                throw new Exception("Unable to find Operating System record in list of available");
            }

            return foundOs.Category;
        }

        public static string GetPowerState(IVirtualMachine vm)
        {
            if (vm != null)
            {
                return vm.PowerState.Value.ToLower().Replace("powerstate/", "");
            }

            return "not found";
        }

        public static string GetOsType(IVirtualMachine vm)
        {
            if (vm != null)
            {
                return vm.OSType.ToString().ToLower();
            }

            return "not found";
        }

        public static bool IsSameRule(VmRuleDto rule1, VmRuleDto rule2)
        {
            if (rule1.Direction == rule2.Direction
                && rule1.Protocol == rule2.Protocol
                && rule1.Ip == rule2.Ip
                 && rule1.Port == rule2.Port)
            {
                return true;
            }
            return false;
        }

        public static int GetNextVmRulePriority(List<VmRuleDto> rules, RuleDirection direction)
        {
            var ruleWithHighestPriority = rules.Where(r=> r.Direction == direction).OrderByDescending(r => r.Priority).FirstOrDefault();

            if (ruleWithHighestPriority == null)
            {
                return 500;
            }
            else if (ruleWithHighestPriority.Priority < 500)
            {
                return 500;
            }
            else
            {
                return ruleWithHighestPriority.Priority + 10;
            }
        }


    }
}
