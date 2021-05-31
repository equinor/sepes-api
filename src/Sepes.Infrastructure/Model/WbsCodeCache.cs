using System;

namespace Sepes.Infrastructure.Model
{
    public class WbsCodeCache
    {
        public WbsCodeCache(string wbsCode, DateTime expires)
        {
            WbsCode = wbsCode;
            Expires = expires;
        }

        public string WbsCode { get; set; }

        public DateTime Expires { get; set; }
    }
}
