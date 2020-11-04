namespace Sepes.Infrastructure.Dto.VirtualMachine
{
    public class VmRuleDto
    {
        public string Id { get; set; }
        public RuleDirection Direction { get; set; }

        public string Description { get; set; }  
        public string Protocol { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }      
    }
}
