using System;

namespace Sepes.Infrastructure.Model
{
    public class WbsCodeCache
    {
        public WbsCodeCache(string wbsCode, bool valid, DateTime expires)
        {
            WbsCode = wbsCode;
            Valid = valid;
            Expires = expires;
        }

        public string WbsCode { get; set; }

        public bool Valid { get; set; }

        public DateTime Expires { get; set; }
    }
}
