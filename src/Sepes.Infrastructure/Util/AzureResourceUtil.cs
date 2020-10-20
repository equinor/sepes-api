using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Service.Azure.Interface;

namespace Sepes.Infrastructure.Util
{
    public static class AzureResourceUtil
    {        

        public static CloudResourceCRUDResult CreateResultFromIResource(IResource resource)
        {
            return new CloudResourceCRUDResult() { Resource = resource, Success = true };
        }   
        
        public static void ThrowIfResourceIsNull(IResource resource, string resourceType, string name, string errorMessagePrefix)
        {
            if(resource == null)
            {
                throw new System.Exception($"{errorMessagePrefix}: Resource {resourceType} with name {name} was not found");
            }
        }

        public static string CreateDescriptionForResourceOperation(string resourceType, string operationType, int sandboxId, int resourceId = 0)
        {
            var description = $"{operationType} {resourceType}";

            if(resourceId > 0)
            {
                description += $" with id {resourceId}";
            }

            description += $" for sandbox {sandboxId}";

            return description;
        }
    }
}
