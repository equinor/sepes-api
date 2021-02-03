namespace Sepes.Infrastructure.Dto
{
    public class FirewallRule
    {
        public string Id { get; set; }

        public string Address { get; set; }
        public int Port { get; set; }

        public FirewallRuleAction Action { get; set; }
    }

    public enum FirewallRuleAction { Allow, Deny }    
}
