using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Util;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Constants
{
    public static class AzureVmConstants
    {
        public const string WINDOWS = "windows";
        public const string LINUX = "linux";

        public const int MIN_RULE_PRIORITY = 500;
        public const int MAX_RULE_PRIORITY = 4000;

        public static class RulePresets
        {
            public const string ALLOW_FOR_SERVICETAG_VNET = "AllowAllForServiceTagVNet";
            public const int ALLOW_FOR_SERVICETAG_VNET_PRIORITY = 4050;
            public const string OPEN_CLOSE_INTERNET = "control-internet-access";

            public static List<VmRuleDto> CreateInitialVmRules(int vmId)
            {
                return new List<VmRuleDto>() { new VmRuleDto() { Name = AzureResourceNameUtil.NsgRuleNameForVm(vmId, OPEN_CLOSE_INTERNET), Description = "Control outbound internet access. Set to Allow or Deny as needed!", Action = RuleAction.Deny, Direction = RuleDirection.Outbound } };
            }

        }
    }

}