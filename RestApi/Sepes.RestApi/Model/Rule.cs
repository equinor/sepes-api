using System.Net;

namespace Sepes.RestApi.Model
{
    public class Rule
    {
        public Rule(ushort port, IPAddress ip)
        {
            this.port = port;
            this.ip = ip;
        }

        public readonly ushort port;
        public readonly IPAddress ip;
    }
}
