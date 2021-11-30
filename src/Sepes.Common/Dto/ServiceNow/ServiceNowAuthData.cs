using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Common.Dto.ServiceNow
{
    public class ServiceNowAuthData
    {
        public string grant_type { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}
