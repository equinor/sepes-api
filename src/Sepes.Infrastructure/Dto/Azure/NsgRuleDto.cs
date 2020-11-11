

using Sepes.Infrastructure.Dto.VirtualMachine;

namespace Sepes.Infrastructure.Dto.Azure
{
    public class NsgRuleDto
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }

        public int SourcePort { get; set; }

        public string SourceAddress { get; set; }

        public string DestinationAddress { get; set; }
        public int DestinationPort { get; set; }
        public string Protocol { get; set; }

        public RuleAction Action { get; set; } = RuleAction.Allow;


    }
}
