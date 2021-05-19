using Microsoft.Azure.Management.Network.Fluent;
using Sepes.Common.Constants;
using System;

namespace Sepes.Azure.Util
{
    public static class AzureVNetUtil
    {
        public static ISubnet GetSandboxSubnet(INetwork network)
        {
            foreach (var curSubnet in network.Subnets)
            {
                if (curSubnet.Value.Name != AzureVNetConstants.BASTION_SUBNET_NAME)
                {
                    return curSubnet.Value;
                }
            }

            return null;
        }

        public static ISubnet GetBastionSubnet(INetwork network)
        {
            return network.Subnets[AzureVNetConstants.BASTION_SUBNET_NAME];          
        }

        public static string GetBastionSubnetId(INetwork network)
        {
            return GetBastionSubnet(network).Inner.Id;
        }

        public static ISubnet GetSandboxSubnetOrThrow(INetwork network)
        {
            var sandboxSubnet = GetSandboxSubnet(network);

            if (sandboxSubnet == null)
            {
                throw new Exception("Could not locate Sandbox subnet");
            }

            return sandboxSubnet;
        }
    }
}
