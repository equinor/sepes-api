using System;
using System.Net;

namespace Sepes.RestApi.Model
{
    public class Rule
    {
        public ushort port { get; }
        public string ip { get; }

        public Rule(ushort port, IPAddress ip)
        {
            this.port = port;
            this.ip = ip.ToString();
        }
        public Rule(ushort port, string ip)
        {
            this.port = port;
            this.ip = IPAddress.Parse(ip).ToString();
        }

        public RuleInput ToRuleInput()
        {
            return new RuleInput(){ port = port, ip = ip };
        }

        public override bool Equals(object obj)
        {
            return obj is Rule rule &&
                   port == rule.port &&
                   ip == rule.ip;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(port, ip);
        }
    }
}
