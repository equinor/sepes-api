using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureVNetService
    {
        private readonly IAzure _azure;

        //TODO: Add Constructor

        public string CreateVNetName(string studyName, string sandboxName)
        {
            return $"vnet-study-{studyName}-{sandboxName}";
        }

        public async Task<string> Create(Region region, string resourceGroup, string studyName, string sandboxName)
        {
            var networkName = CreateVNetName(studyName, sandboxName);           

            var addressSpace = "10.100.10.0/23"; // Until 10.100.11.255 Can have 512 adresses, but must reserve some;

            var bastionSubnetName = "AzureBastionSubnet";
            var bastionSubnetAddress = "10.100.0.0/24"; //Can only use 256 adress, so max is 10.100.0.255

            var sandboxSubnetName = $"snet-{sandboxName}";
            var sandboxSubnetAddress = "10.100.1.0/24";

            var network = await _azure.Networks.Define(networkName)
                .WithRegion(region)
                .WithExistingResourceGroup(resourceGroup)
                .WithAddressSpace(addressSpace)
                .WithSubnet(bastionSubnetName, bastionSubnetAddress)
                .WithSubnet(sandboxSubnetName, sandboxSubnetAddress)
                .CreateAsync();

            return network.Id;
        }

        public async Task Delete(string resourceGroup, string vNetName)
        {
            await _azure.Networks.DeleteByResourceGroupAsync(resourceGroup, vNetName);
            return;
        }     

    }
}
