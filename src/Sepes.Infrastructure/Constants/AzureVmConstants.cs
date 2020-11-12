using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Util;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Constants
{
    public static class AzureVmConstants
    {
        public const string WINDOWS = "windows";
        public const string LINUX = "linux";

        public static class RulePresets
        {
            public const string ALLOW_FOR_SERVICETAG_VNET = "AllowAllForServiceTagVNet";
            public const string OPEN_CLOSE_INTERNET = "control-internet-access";

            public static List<VmRuleDto> CreateInitialVmRules(string vmName)
            {
                return new List<VmRuleDto>() { new VmRuleDto() { Name = AzureResourceNameUtil.NsgRuleNameForVm(vmName, OPEN_CLOSE_INTERNET), Description = "Control outbound internet access. Set to Allow or Deny as needed!", Action = RuleAction.Deny, Priority = 1000, Direction = RuleDirection.Outbound } };
            }

        }
    }

}