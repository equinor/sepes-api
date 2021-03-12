using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sepes.Infrastructure.Util
{
    public static class CloudResourceUtil
    {
        public static CloudResource GetSibilingResource(CloudResource resource, string resourceType)
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

        public static CloudResourceDto GetResourceByType(List<CloudResourceDto> resources, string resourceType, bool mustBeSandboxControlled = false)
        {
            if(resources == null)
            {
                throw new ArgumentNullException("resources");
            }

            return resources.FirstOrDefault(r => r.ResourceType == resourceType && (!mustBeSandboxControlled || (mustBeSandboxControlled && r.SandboxControlled)));         
        }

        public static CloudResource GetResourceByType(List<CloudResource> resources, string resourceType, bool mustBeSandboxControlled = false)
        {
            if (resources == null)
            {
                throw new ArgumentNullException("resources");
            }

            return resources.FirstOrDefault(r => r.ResourceType == resourceType && (!mustBeSandboxControlled || (mustBeSandboxControlled && r.SandboxControlled)));
        }

        public static CloudResource GetResourceByTypeAndPurpose(List<CloudResource> resources, string resourceType, string purpose)
        {
            if (resources == null)
            {
                throw new ArgumentNullException("resources");
            }

            return resources.FirstOrDefault(r => r.ResourceType == resourceType && r.Purpose == purpose);
        }

        public static CloudResource GetSandboxResourceGroupEntry(List<CloudResource> resources)
        {         
            if (resources == null)
            {
                throw new ArgumentNullException("resources");
            }

            return resources.FirstOrDefault(r => r.ResourceType == AzureResourceType.ResourceGroup && (r.SandboxControlled || r.Purpose == CloudResourcePurpose.SandboxResourceGroup));          
        }

        public static List<CloudResource> GetSandboxResourceGroupsForStudy(Study study)
        {
            return study.Sandboxes
                .Where(sb => !SoftDeleteUtil.IsMarkedAsDeleted(sb))
                .Select(sb => GetSandboxResourceGroupEntry(sb.Resources))
                .Where(r => !r.Deleted)
                .ToList();
        }

        public static List<CloudResource> GetDatasetResourceGroupsForStudy(Study study)
        {
            return study.Resources
                .Where(r => !SoftDeleteUtil.IsMarkedAsDeleted(r) && r.ResourceType == AzureResourceType.ResourceGroup && r.Purpose == CloudResourcePurpose.StudySpecificDatasetContainer)
                .ToList();
        }

        public static List<CloudResourceDto> GetAllResourcesByType(List<CloudResourceDto> resources, string resourceType, bool mustBeSandboxControlled = false)
        {
            if (resources == null)
            {
                throw new ArgumentNullException("resources");
            }

            return resources.Where(r => r.ResourceType == resourceType && (!mustBeSandboxControlled || (mustBeSandboxControlled && r.SandboxControlled))).ToList();
        }               
    }
}
