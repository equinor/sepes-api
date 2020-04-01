using System;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;

namespace Sepes.Infrastructure.Dto
{
    public class AzureConfig
    {
        public AzureCredentials credentials { get; }
        public string commonGroup { get; }
        public string joinNetworkRoleName { get; }

        public AzureConfig(string tenant, string client, string secret, string subscription, string commonGroup, string joinNetworkRoleName)
        {
            credentials = new AzureCredentialsFactory().FromServicePrincipal(client, secret, tenant, AzureEnvironment.AzureGlobalCloud).WithDefaultSubscription(subscription);
            this.commonGroup = commonGroup;
            this.joinNetworkRoleName = joinNetworkRoleName;
        }


    }
}