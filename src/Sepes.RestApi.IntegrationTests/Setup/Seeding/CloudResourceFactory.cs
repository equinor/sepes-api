using Sepes.Infrastructure.Model;
using System;

namespace Sepes.RestApi.IntegrationTests.Setup.Seeding
{
    public static class CloudResourceFactory
    {
        public static CloudResource Create(string region, string resourceGroup, string resourceId, string resourceKey, string resourceName, string purpose = null)
        {
           return new CloudResource()
            {
                Region = region,
                ResourceGroupName = resourceGroup, 
                Purpose = purpose,
                ResourceId = resourceId,
                ResourceKey = resourceKey,
                ResourceName = resourceName,
                CreatedBy = "seed",
                Created = DateTime.UtcNow,
                UpdatedBy = "seed",
                Updated = DateTime.UtcNow           
                
            }; 
        }
      
    }
}
