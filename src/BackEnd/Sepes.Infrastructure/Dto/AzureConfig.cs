using System;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;

namespace Sepes.Infrastructure.Dto
{
    public class AzureConfig
    {
        public AzureCredentials Credentials { get; }
        public string CommonGroup { get; }
        public string JoinNetworkRoleName { get; }       

        public AzureConfig(string tenant, string client, string secret, string subscription, string commonGroup, string joinNetworkRoleName)
        {
            Credentials = new AzureCredentialsFactory().FromServicePrincipal(client, secret, tenant, AzureEnvironment.AzureGlobalCloud).WithDefaultSubscription(subscription);
            this.CommonGroup = commonGroup;
            this.JoinNetworkRoleName = joinNetworkRoleName;
        }


    }
}