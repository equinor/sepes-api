using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sepes.Infrastructure.Util
{
    public static class SandboxResourceUtil
    {
        public static SandboxResource GetSibilingResource(SandboxResource resource, string resourceType)
        {
            if (resource.Sandbox == null)
            {
                throw new NullReferenceException($"Cannot navigate to Sandbox for resource {resource.Id}");
            }

            if (resource.Sandbox.Resources == null)
            {
                throw new NullReferenceException($"Cannot navigate to Sandbox sibling resources for resource {resource.Id}");
            }

            return resource.Sandbox.Resources.FirstOrDefault(r => r.ResourceType == resourceType);
        }

        public static SandboxResourceDto GetResourceByType(List<SandboxResourceDto> resources, string resourceType, bool mustBeSandboxControlled = false)
        {
            if(resources == null)
            {
                throw new ArgumentNullException("resources");
            }

            foreach(var curResource in resources)
            {
                if(curResource.ResourceType == resourceType &&
                    (mustBeSandboxControlled == false || (mustBeSandboxControlled && curResource.SandboxControlled) ))
                {
                    return curResource;
                }
            }

            return null;
        }

        public static List<SandboxResourceDto> GetAllResourcesByType(List<SandboxResourceDto> resources, string resourceType, bool mustBeSandboxControlled = false)
        {
            if (resources == null)
            {
                throw new ArgumentNullException("resources");
            }

            var result = new List<SandboxResourceDto>();

            foreach (var curResource in resources)
            {
                if (curResource.ResourceType == resourceType &&
                    (mustBeSandboxControlled == false || (mustBeSandboxControlled && curResource.SandboxControlled)))
                {
                    result.Add(curResource);
                }
            }

            return result;
        }
    }
}
