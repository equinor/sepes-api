using Sepes.Infrastructure.Dto.VirtualMachine;
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
            public const string OPEN_CLOSE_INTERNET = "controlinternetaccess";

            public static List<VmRuleDto> InitialVmRules =  new List<VmRuleDto>() { new VmRuleDto() { Id = AzureVmConstants.RulePresets.OPEN_CLOSE_INTERNET, Description = "Deny outbound internet access", Action = RuleAction.Deny, Priority = 1000, Direction = RuleDirection.Outbound }
        };

        public static Dictionary<string, VmRuleDto> Presets = new Dictionary<string, VmRuleDto>() { { OPEN_CLOSE_INTERNET, new VmRuleDto() { Direction = RuleDirection.Inbound, Action = RuleAction.Allow, Description = "Open internet"  } } };
        }
    }
}
