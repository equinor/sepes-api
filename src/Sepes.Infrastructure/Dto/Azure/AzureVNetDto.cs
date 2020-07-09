using Microsoft.Azure.Management.Network.Fluent;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Dto
{
    public class AzureVNetDto
    {
        public string BastionSubnetId { get
            {
                var subnet = Network.Subnets["AzureBastionSubnet"];
                return subnet.Inner.Id;
            }
        }

        public string Name {
            get
            {
                return Network.Name;
            }
        }

        public INetwork Network{ get; set; }
    }
}
