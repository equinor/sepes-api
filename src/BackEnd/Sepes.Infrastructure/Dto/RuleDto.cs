using Sepes.Infrastructure.Model.SepesSqlModels;
using System;
using System.Net;

namespace Sepes.Infrastructure.Dto
{
    public class RuleDto
    {
        public ushort port { get; }
        public string ip { get; }

        public RuleDto(ushort port, IPAddress ip)
        {
            this.port = port;
            this.ip = ip.ToString();
        }
        public RuleDto(ushort port, string ip)
        {
            this.port = port;
            this.ip = IPAddress.Parse(ip).ToString();
        }

        public RuleInputDto ToRuleInput()
        {
            return new RuleInputDto(){ port = port, ip = ip };
        }

        public RuleDB ToRuleDB()
        {
            return new RuleDB(){ port = port, ip = ip };
        }

        public override bool Equals(object obj)
        {
            return obj is RuleDto rule &&
                   port == rule.port &&
                   ip == rule.ip;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(port, ip);
        }
    }
}
