namespace Sepes.Infrastructure.Dto
{
    public class FirewallRule
    {
        public string Id { get; set; }

        public string Address { get; set; }
        public int Port { get; set; }

        public RuleAction Action { get; set; }
    }

    public enum RuleAction { Allow, Deny }    
}
