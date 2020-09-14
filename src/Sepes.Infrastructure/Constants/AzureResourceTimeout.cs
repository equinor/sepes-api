using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Constants
{
    public class AzureResourceTimeout
    {
        public static int RESOURCE_GROUP = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
    }
}
