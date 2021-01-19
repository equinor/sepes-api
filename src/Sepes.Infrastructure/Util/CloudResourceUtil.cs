﻿using Sepes.Infrastructure.Constants;
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

        public static CloudResource GetSandboxResourceGroupEntry(List<CloudResource> resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException("resources");
            }

            foreach (var curResource in resources)
            {
                if (curResource.ResourceType == AzureResourceType.ResourceGroup && curResource.SandboxControlled)
                {
                    return curResource;
                }
            }

            return null;
        }

        public static List<CloudResource> GetResourceGroups(Study study)
        {
            return study.Sandboxes
                .Where(sb => !SoftDeleteUtil.IsMarkedAsDeleted(sb))
                .Select(sb => GetSandboxResourceGroupEntry(sb.Resources))
                .Where(r => !r.Deleted.HasValue)
                .ToList();
        }

        public static List<CloudResourceDto> GetAllResourcesByType(List<CloudResourceDto> resources, string resourceType, bool mustBeSandboxControlled = false)
        {
            if (resources == null)
            {
                throw new ArgumentNullException("resources");
            }

            var result = new List<CloudResourceDto>();

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

        public static bool IsDeleted(CloudResourceDto resource)
        {
            return resource.Deleted.HasValue;
        }       
    }
}
