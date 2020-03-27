namespace Sepes.RestApi.Model
{
    public class RuleInput
    {
        public ushort port { get; set; }
        public string ip { get; set; }

        public Rule ToRule()
        {
            return new Rule(port, ip);
        }
    }
}
