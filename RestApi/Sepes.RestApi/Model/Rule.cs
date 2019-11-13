using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Sepes.RestApi.Model
{
    public class Rule
    {
        public readonly ushort port;
        public readonly IPAddress ip;

        public Rule(ushort port, IPAddress ip)
        {
            this.port = port;
            this.ip = ip;
        }

        public override bool Equals(object obj)
        {
            return obj is Rule rule &&
                   port == rule.port &&
                   ip.Equals(rule.ip);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(port, ip);
        }
    }
}
