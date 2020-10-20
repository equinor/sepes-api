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
    }
}
