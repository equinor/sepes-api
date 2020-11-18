using Sepes.Infrastructure.Model;
using System;
using System.Linq;

namespace Sepes.Infrastructure.Util
{
    public static class SandboxResourceUtil
    {
        public static SandboxResource GetSibilingResource(SandboxResource resource, string resourceType)
        {
            if(resource.Sandbox == null)
            {
                throw new NullReferenceException($"Cannot navigate to Sandbox for resource {resource.Id}");
            }

            if (resource.Sandbox.Resources == null)
            {
                throw new NullReferenceException($"Cannot navigate to Sandbox sibling resources for resource {resource.Id}");
            }

           return resource.Sandbox.Resources.FirstOrDefault(r => r.ResourceType == resourceType);   
        }
    }
}
