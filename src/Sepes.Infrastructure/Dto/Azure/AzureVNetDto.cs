using Microsoft.Azure.Management.Network.Fluent;

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

        public string SandboxSubnetName
        {
            get;set;
        }

        public string Name {
            get
            {
                return Network.Name;
            }
        }

        public string Key
        {
            get
            {
                return Network.Key;
            }
        }

        public string Id
        {
            get
            {
                return Network.Id;
            }
        }
      

        public INetwork Network{ get; set; }
    }
}
